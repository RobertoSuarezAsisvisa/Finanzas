using FinanzasMCP.Domain.CreditCards;

namespace FinanzasMCP.Domain.Tests;

public sealed class CreditCardAccountTests
{
    [Fact]
    public void Purchase_increases_outstanding_balance_and_reduces_available_credit()
    {
        var card = CreateCard();

        card.RegisterCharge(75m);

        Assert.Equal(75m, card.OutstandingBalance);
        Assert.Equal(425m, card.AvailableCredit);
    }

    [Fact]
    public void Payment_reduces_outstanding_balance_and_restores_available_credit()
    {
        var card = CreateCard();
        card.RegisterCharge(120m);

        card.RegisterPayment(50m);

        Assert.Equal(70m, card.OutstandingBalance);
        Assert.Equal(430m, card.AvailableCredit);
    }

    [Fact]
    public void Refund_cannot_make_outstanding_balance_negative()
    {
        var card = CreateCard();
        card.RegisterCharge(30m);

        card.RegisterPayment(50m);

        Assert.Equal(0m, card.OutstandingBalance);
        Assert.Equal(500m, card.AvailableCredit);
    }

    [Fact]
    public void Charge_cannot_exceed_credit_limit()
    {
        var card = CreateCard();

        var exception = Assert.Throws<InvalidOperationException>(() => card.RegisterCharge(501m));

        Assert.Equal("Credit card limit exceeded.", exception.Message);
    }

    private static CreditCardAccount CreateCard()
        => CreditCardAccount.Create(
            Guid.NewGuid(),
            "Banco Diners Club del Ecuador",
            CreditCardBrand.Discover,
            500m,
            1,
            15,
            rewardsProgram: "Miles",
            interestNominalAnnual: 15.60m,
            interestEffectiveAnnual: 16.77m);
}
