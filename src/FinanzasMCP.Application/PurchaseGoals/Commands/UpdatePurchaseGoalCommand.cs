using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Application.PurchaseGoals.Commands;

public sealed record UpdatePurchaseGoalCommand(Guid Id, string Name, decimal TargetPrice, string? Description, int Priority, string? Url, Guid? AccountId, DateTimeOffset? TargetDate, PurchaseGoalStatus? Status, DateTimeOffset? PurchasedAt);
