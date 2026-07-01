using FinanzasMCP.Domain.Transactions;
using FinanzasMCP.Domain.CreditCards;

namespace FinanzasMCP.Application.Transactions.Commands;

public sealed record UpdateTransactionCommand(
    Guid Id,
    TransactionType Type,
    decimal Amount,
    string Currency,
    Guid AccountId,
    Guid? ToAccountId,
    Guid? CategoryId,
    Guid? BudgetId,
    string? Description,
    string? Reference,
    DateTimeOffset TransactionDate,
    Guid? RecurringRuleId,
    IReadOnlyList<Guid> TagIds,
    CreditCardOperationType? CreditCardOperationType,
    Guid? CreditCardStatementId,
    bool IsForeignCreditCardTransaction,
    int? InstallmentCount,
    string? Merchant);
