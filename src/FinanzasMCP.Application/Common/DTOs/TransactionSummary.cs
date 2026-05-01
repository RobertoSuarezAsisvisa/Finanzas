using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record TransactionSummary(
    Guid Id,
    TransactionType Type,
    decimal Amount,
    string Currency,
    Guid AccountId,
    Guid? ToAccountId,
    Guid? CategoryId,
    string? Description,
    string? Reference,
    DateTimeOffset TransactionDate);
