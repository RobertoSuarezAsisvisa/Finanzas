namespace FinanzasMCP.Application.Accounts.Commands;

public sealed record UpdateAccountCommand(Guid Id, string Name, string Currency, string? BankName, string? AccountNumber, string? Provider, bool IsActive);
