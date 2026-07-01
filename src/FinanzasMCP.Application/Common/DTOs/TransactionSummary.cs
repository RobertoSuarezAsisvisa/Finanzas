using FinanzasMCP.Domain.Transactions;
using FinanzasMCP.Domain.CreditCards;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record TransactionSummary(
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
    IReadOnlyList<Guid> TagIds,
    int AttachmentCount,
    Guid? CreditCardAccountId = null,
    CreditCardOperationType? CreditCardOperationType = null,
    Guid? CreditCardStatementId = null,
    bool IsForeignCreditCardTransaction = false,
    int? InstallmentCount = null,
    string? Merchant = null);
