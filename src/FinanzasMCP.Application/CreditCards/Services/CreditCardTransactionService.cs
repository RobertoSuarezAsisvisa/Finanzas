using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.CreditCards;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.CreditCards.Services;

public sealed class CreditCardTransactionService(IFinanzasMCPDbContext dbContext)
{
    public async Task ApplyAsync(
        Transaction transaction,
        Account account,
        Account? toAccount,
        CreditCardOperationType? requestedOperationType,
        Guid? statementId,
        bool isForeign,
        int? installmentCount,
        string? merchant,
        CancellationToken cancellationToken)
    {
        var operationType = ResolveOperationType(transaction.Type, account, toAccount, requestedOperationType);
        if (operationType is null)
        {
            ApplyCashMovement(transaction.Type, account, toAccount, transaction.Amount);
            return;
        }

        var creditCard = await ResolveCreditCardAsync(operationType.Value, account, toAccount, cancellationToken);
        ApplyCreditCardMovement(operationType.Value, creditCard, transaction.Type, account, toAccount, transaction.Amount);

        var metadata = await dbContext.CreditCardTransactions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.TransactionId == transaction.Id, cancellationToken);

        if (metadata is null)
        {
            dbContext.CreditCardTransactions.Add(CreditCardTransaction.Create(
                transaction.Id,
                creditCard.Id,
                operationType.Value,
                statementId,
                isForeign,
                installmentCount,
                merchant));
        }
        else
        {
            metadata.Restore();
            metadata.UpdateDetails(creditCard.Id, operationType.Value, statementId, isForeign, installmentCount, merchant);
        }
    }

    public async Task ReverseAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        var metadata = await dbContext.CreditCardTransactions
            .FirstOrDefaultAsync(x => x.TransactionId == transaction.Id, cancellationToken);

        if (metadata is null)
        {
            ReverseCashMovement(transaction);
            return;
        }

        var creditCard = await dbContext.CreditCardAccounts
            .FirstAsync(x => x.Id == metadata.CreditCardAccountId, cancellationToken);

        ReverseCreditCardMovement(metadata.OperationType, creditCard, transaction);
        metadata.SoftDelete();
    }

    private static CreditCardOperationType? ResolveOperationType(
        TransactionType type,
        Account account,
        Account? toAccount,
        CreditCardOperationType? requestedOperationType)
    {
        if (requestedOperationType is not null)
        {
            return requestedOperationType.Value;
        }

        if (account.AccountType == AccountType.CreditCard)
        {
            return type switch
            {
                TransactionType.Expense => CreditCardOperationType.Purchase,
                TransactionType.Income => CreditCardOperationType.Refund,
                _ => null
            };
        }

        if (type == TransactionType.Transfer && toAccount?.AccountType == AccountType.CreditCard)
        {
            return CreditCardOperationType.Payment;
        }

        return null;
    }

    private async Task<CreditCardAccount> ResolveCreditCardAsync(
        CreditCardOperationType operationType,
        Account account,
        Account? toAccount,
        CancellationToken cancellationToken)
    {
        var accountId = operationType == CreditCardOperationType.Payment ? toAccount?.Id : account.Id;
        if (accountId is null)
        {
            throw new InvalidOperationException("Credit card destination account is required.");
        }

        return await dbContext.CreditCardAccounts
            .FirstAsync(x => x.AccountId == accountId.Value, cancellationToken);
    }

    private static void ApplyCreditCardMovement(
        CreditCardOperationType operationType,
        CreditCardAccount creditCard,
        TransactionType transactionType,
        Account account,
        Account? toAccount,
        decimal amount)
    {
        switch (operationType)
        {
            case CreditCardOperationType.Purchase:
            case CreditCardOperationType.Fee:
            case CreditCardOperationType.Interest:
                creditCard.RegisterCharge(amount);
                break;
            case CreditCardOperationType.CashAdvance:
                creditCard.RegisterCharge(amount);
                if (toAccount is not null && toAccount.AccountType != AccountType.CreditCard)
                {
                    toAccount.Deposit(amount);
                }
                break;
            case CreditCardOperationType.Payment:
                if (account.AccountType == AccountType.CreditCard)
                {
                    throw new InvalidOperationException("Credit card payments require a cash, bank, or wallet source account.");
                }
                account.Withdraw(amount);
                creditCard.RegisterPayment(amount);
                break;
            case CreditCardOperationType.Refund:
                creditCard.RegisterPayment(amount);
                break;
            default:
                ApplyCashMovement(transactionType, account, toAccount, amount);
                break;
        }
    }

    private static void ReverseCreditCardMovement(CreditCardOperationType operationType, CreditCardAccount creditCard, Transaction transaction)
    {
        switch (operationType)
        {
            case CreditCardOperationType.Purchase:
            case CreditCardOperationType.Fee:
            case CreditCardOperationType.Interest:
                creditCard.RegisterPayment(transaction.Amount);
                break;
            case CreditCardOperationType.CashAdvance:
                creditCard.RegisterPayment(transaction.Amount);
                transaction.ToAccount?.Withdraw(transaction.Amount);
                break;
            case CreditCardOperationType.Payment:
                transaction.Account.Deposit(transaction.Amount);
                creditCard.RegisterCharge(transaction.Amount);
                break;
            case CreditCardOperationType.Refund:
                creditCard.RegisterCharge(transaction.Amount);
                break;
        }
    }

    private static void ApplyCashMovement(TransactionType transactionType, Account account, Account? toAccount, decimal amount)
    {
        switch (transactionType)
        {
            case TransactionType.Income:
                account.Deposit(amount);
                break;
            case TransactionType.Expense:
                account.Withdraw(amount);
                break;
            case TransactionType.Transfer:
                account.Withdraw(amount);
                toAccount?.Deposit(amount);
                break;
        }
    }

    private static void ReverseCashMovement(Transaction transaction)
    {
        switch (transaction.Type)
        {
            case TransactionType.Income:
                transaction.Account.Withdraw(transaction.Amount);
                break;
            case TransactionType.Expense:
                transaction.Account.Deposit(transaction.Amount);
                break;
            case TransactionType.Transfer:
                transaction.Account.Deposit(transaction.Amount);
                transaction.ToAccount?.Withdraw(transaction.Amount);
                break;
        }
    }
}
