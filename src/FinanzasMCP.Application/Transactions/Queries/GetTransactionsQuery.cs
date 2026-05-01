namespace FinanzasMCP.Application.Transactions.Queries;

public sealed record GetTransactionsQuery(Guid? AccountId = null);
