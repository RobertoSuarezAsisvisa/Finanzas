using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.Application.Transactions.Queries;

public sealed record GetTransactionsQuery(
    Guid? AccountId = null,
    TransactionType? Type = null,
    Guid? CategoryId = null,
    Guid? BudgetId = null,
    DateTimeOffset? DateFrom = null,
    DateTimeOffset? DateTo = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 10);
