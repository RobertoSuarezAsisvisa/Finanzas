namespace FinanzasMCP.Application.PurchaseGoals.Commands;

public sealed record CreatePurchaseGoalCommand(string Name, decimal TargetPrice, string? Description, int Priority, string? Url, Guid? AccountId, DateTimeOffset? TargetDate);
