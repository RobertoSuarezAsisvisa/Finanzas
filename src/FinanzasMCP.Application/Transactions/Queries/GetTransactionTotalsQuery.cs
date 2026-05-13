using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.Application.Transactions.Queries;

public sealed record GetTransactionTotalsQuery(
    Guid? AccountId = null,
    TransactionType? Type = null,
    Guid? CategoryId = null,
    DateTimeOffset? DateFrom = null,
    DateTimeOffset? DateTo = null,
    string? Search = null);
