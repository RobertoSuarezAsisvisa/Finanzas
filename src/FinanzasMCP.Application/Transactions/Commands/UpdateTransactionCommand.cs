using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.Application.Transactions.Commands;

public sealed record UpdateTransactionCommand(
    Guid Id,
    TransactionType Type,
    decimal Amount,
    string Currency,
    Guid AccountId,
    Guid? ToAccountId,
    Guid? CategoryId,
    string? Description,
    string? Reference,
    DateTimeOffset TransactionDate,
    Guid? RecurringRuleId,
    IReadOnlyList<Guid> TagIds);
