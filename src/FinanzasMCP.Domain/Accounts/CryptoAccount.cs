using FinanzasMCP.Domain.Common;
using FinanzasMCP.Domain.Crypto;

namespace FinanzasMCP.Domain.Accounts;

public sealed class CryptoAccount : SoftDeletableEntity
{
    public Guid AccountId { get; private set; }
    public string Symbol { get; private set; } = string.Empty;
    public string? Network { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal? AvgBuyPriceUsd { get; private set; }
    public Account Account { get; private set; } = null!;
    
    public ICollection<CryptoLot> Lots { get; private set; } = new List<CryptoLot>();

    public static CryptoAccount Create(
        Guid accountId,
        string symbol,
        string? network = null,
        decimal quantity = 0m,
        decimal? avgBuyPriceUsd = null)
    {
        return new CryptoAccount
        {
            AccountId = accountId,
            Symbol = symbol.Trim().ToUpperInvariant(),
            Network = network?.Trim(),
            Quantity = quantity,
            AvgBuyPriceUsd = avgBuyPriceUsd
        };
    }

    public void UpdateDetails(
        Guid accountId,
        string symbol,
        string? network = null,
        decimal quantity = 0m,
        decimal? avgBuyPriceUsd = null)
    {
        AccountId = accountId;
        Symbol = symbol.Trim().ToUpperInvariant();
        Network = network?.Trim();
        Quantity = quantity;
        AvgBuyPriceUsd = avgBuyPriceUsd;
        MarkUpdated();
    }
}
