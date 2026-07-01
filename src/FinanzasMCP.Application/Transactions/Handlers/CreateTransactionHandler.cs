using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.CreditCards.Services;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Transactions.Commands;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Budgets;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Transactions.Handlers;

public sealed class CreateTransactionHandler(IFinanzasMCPDbContext dbContext, CreditCardTransactionService creditCardTransactionService)
{
    public async Task<TransactionSummary> Handle(CreateTransactionCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            throw new InvalidOperationException("Amount must be positive.");
        }

        if (command.Type == TransactionType.Transfer && command.ToAccountId is null)
        {
            throw new InvalidOperationException("Destination account is required for transfers.");
        }

        if (command.Type == TransactionType.Expense && command.CategoryId is null)
        {
            throw new InvalidOperationException("Expense transactions require a category.");
        }

        await ValidateBudgetAsync(command.Type, command.BudgetId, cancellationToken);

        var account = await dbContext.Accounts.FirstAsync(x => x.Id == command.AccountId, cancellationToken);
        Account? toAccount = null;
        if (command.ToAccountId is not null)
        {
            toAccount = await dbContext.Accounts.FirstAsync(x => x.Id == command.ToAccountId, cancellationToken);
        }

        var transaction = Transaction.Create(
            command.Type,
            command.Amount,
            command.Currency,
            command.AccountId,
            command.ToAccountId,
            command.CategoryId,
            command.BudgetId,
            command.Description,
            command.Reference,
            command.TransactionDate.ToUtcSafe(),
            command.RecurringRuleId);

        dbContext.Set<Transaction>().Add(transaction);
        await creditCardTransactionService.ApplyAsync(
            transaction,
            account,
            toAccount,
            command.CreditCardOperationType,
            command.CreditCardStatementId,
            command.IsForeignCreditCardTransaction,
            command.InstallmentCount,
            command.Merchant,
            cancellationToken);

        transaction.ReplaceTags(command.TagIds);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TransactionSummary(
            transaction.Id,
            transaction.Type,
            transaction.Amount,
            transaction.Currency,
            transaction.AccountId,
            transaction.ToAccountId,
            transaction.CategoryId,
            transaction.BudgetId,
            transaction.Description,
            transaction.Reference,
            transaction.TransactionDate,
            transaction.Tags.Select(tag => tag.TagId).ToArray(),
            0,
            null,
            command.CreditCardOperationType,
            command.CreditCardStatementId,
            command.IsForeignCreditCardTransaction,
            command.InstallmentCount,
            command.Merchant);
    }

    private async Task ValidateBudgetAsync(TransactionType type, Guid? budgetId, CancellationToken cancellationToken)
    {
        if (budgetId is null)
        {
            return;
        }

        if (type != TransactionType.Expense)
        {
            throw new InvalidOperationException("Only expense transactions can be assigned to a budget.");
        }

        var budget = await dbContext.Set<Budget>().FirstAsync(x => x.Id == budgetId.Value, cancellationToken);
        if (!budget.IsActive)
        {
            throw new InvalidOperationException("The selected budget is inactive.");
        }
    }
}
