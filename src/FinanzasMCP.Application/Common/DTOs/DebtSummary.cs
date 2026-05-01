using FinanzasMCP.Domain.Debts;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record DebtSummary(Guid Id, DebtType Type, string ContactName, decimal OriginalAmount, decimal RemainingAmount, string Currency, DateTimeOffset? DueDate, Guid? AccountId, DebtStatus Status, string? Notes);
