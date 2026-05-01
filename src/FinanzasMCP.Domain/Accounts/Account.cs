using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Accounts;

public sealed class Account : SoftDeletableEntity
{
    public string Name { get; private set; } = string.Empty;
    public AccountType AccountType { get; private set; }
    public string Currency { get; private set; } = "USD";
    public decimal Balance { get; private set; }
    public string? BankName { get; private set; }
    public string? AccountNumber { get; private set; }
    public string? Provider { get; private set; }
    public bool IsActive { get; private set; } = true;
    public CryptoAccount? CryptoAccount { get; private set; }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new InvalidOperationException("Amount must be positive.");
        Balance += amount;
        MarkUpdated();
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new InvalidOperationException("Amount must be positive.");
        if (Balance < amount) throw new InvalidOperationException("Insufficient balance.");
        Balance -= amount;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkUpdated();
    }

    public void UpdateDetails(
        string name,
        string currency,
        string? bankName = null,
        string? accountNumber = null,
        string? provider = null)
    {
        Name = name.Trim();
        Currency = currency.Trim().ToUpperInvariant();
        BankName = bankName?.Trim();
        AccountNumber = accountNumber?.Trim();
        Provider = provider?.Trim();
        MarkUpdated();
    }

    public static Account Create(
        string name,
        AccountType accountType,
        string currency = "USD",
        decimal balance = 0m,
        string? bankName = null,
        string? accountNumber = null,
        string? provider = null)
    {
        return new Account
        {
            Name = name.Trim(),
            AccountType = accountType,
            Currency = currency.Trim().ToUpperInvariant(),
            Balance = balance,
            BankName = bankName?.Trim(),
            AccountNumber = accountNumber?.Trim(),
            Provider = provider?.Trim(),
            IsActive = true
        };
    }
}
