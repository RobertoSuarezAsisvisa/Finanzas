using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.Application.Goals.Handlers;

internal static class TransactionBalanceAdjustment
{
    public static void Reverse(Transaction transaction)
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

    public static void Apply(Transaction transaction, Account account, Account? toAccount)
    {
        switch (transaction.Type)
        {
            case TransactionType.Income:
                account.Deposit(transaction.Amount);
                break;
            case TransactionType.Expense:
                account.Withdraw(transaction.Amount);
                break;
            case TransactionType.Transfer:
                account.Withdraw(transaction.Amount);
                toAccount?.Deposit(transaction.Amount);
                break;
        }
    }
}
