using FinanzasMCP.Domain.Shopping;

namespace FinanzasMCP.Domain.Tests;

public sealed class ShoppingPriceTests
{
    [Fact]
    public void Store_product_price_calculates_unit_price()
    {
        var price = StoreProductPrice.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            2.50m,
            500m,
            ShoppingUnit.Gram,
            DateTimeOffset.UtcNow,
            StoreProductPriceSource.Manual);

        Assert.Equal(0.005m, price.UnitPrice);
    }

    [Fact]
    public void Store_product_price_rejects_zero_quantity()
    {
        Assert.Throws<InvalidOperationException>(() => StoreProductPrice.CalculateUnitPrice(1m, 0m));
    }
}
