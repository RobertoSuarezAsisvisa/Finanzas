using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.CreditCards.Commands;
using FinanzasMCP.Application.CreditCards.Handlers;
using FinanzasMCP.Application.CreditCards.Queries;
using FinanzasMCP.Domain.CreditCards;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class CreditCardTools(
    CreateCreditCardHandler createCreditCardHandler,
    GetCreditCardsHandler getCreditCardsHandler,
    UpdateCreditCardHandler updateCreditCardHandler,
    DeleteCreditCardHandler deleteCreditCardHandler,
    CloseCreditCardStatementHandler closeCreditCardStatementHandler,
    GetCreditCardStatementsHandler getCreditCardStatementsHandler)
{
    [McpServerTool, System.ComponentModel.Description("Creates a credit card account with a credit limit and billing configuration.")]
    public Task<CreditCardSummary> CreateCreditCard(
        string name,
        string issuer,
        CreditCardBrand brand,
        decimal creditLimit = 500m,
        string currency = "USD",
        int statementClosingDay = 1,
        int paymentDueDay = 15,
        string? productName = null,
        string? lastFour = null,
        decimal outstandingBalance = 0m,
        CreditCardPaymentMode paymentMode = CreditCardPaymentMode.Manual,
        string? rewardsProgram = "Miles",
        CreditCardStatementDelivery statementDelivery = CreditCardStatementDelivery.Virtual,
        decimal? interestNominalAnnual = 15.60m,
        decimal? interestEffectiveAnnual = 16.77m,
        CancellationToken cancellationToken = default)
        => createCreditCardHandler.Handle(new CreateCreditCardCommand(name, currency, issuer, brand, creditLimit, statementClosingDay, paymentDueDay, productName, lastFour, outstandingBalance, paymentMode, rewardsProgram, statementDelivery, interestNominalAnnual, interestEffectiveAnnual), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists credit cards and their available credit.")]
    public Task<IReadOnlyList<CreditCardSummary>> ListCreditCards(CancellationToken cancellationToken = default)
        => getCreditCardsHandler.Handle(new GetCreditCardsQuery(), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a credit card account.")]
    public Task<CreditCardSummary> UpdateCreditCard(
        Guid id,
        string name,
        string issuer,
        CreditCardBrand brand,
        decimal creditLimit,
        string currency = "USD",
        int statementClosingDay = 1,
        int paymentDueDay = 15,
        string? productName = null,
        string? lastFour = null,
        CreditCardPaymentMode paymentMode = CreditCardPaymentMode.Manual,
        string? rewardsProgram = "Miles",
        CreditCardStatementDelivery statementDelivery = CreditCardStatementDelivery.Virtual,
        decimal? interestNominalAnnual = 15.60m,
        decimal? interestEffectiveAnnual = 16.77m,
        bool isActive = true,
        CancellationToken cancellationToken = default)
        => updateCreditCardHandler.Handle(new UpdateCreditCardCommand(id, name, currency, issuer, brand, creditLimit, statementClosingDay, paymentDueDay, productName, lastFour, paymentMode, rewardsProgram, statementDelivery, interestNominalAnnual, interestEffectiveAnnual, isActive), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a credit card account when it has no outstanding balance.")]
    public Task DeleteCreditCard(Guid id, CancellationToken cancellationToken = default)
        => deleteCreditCardHandler.Handle(new DeleteCreditCardCommand(id), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Closes a credit card statement manually for a period.")]
    public Task<CreditCardStatementSummary> CloseCreditCardStatement(
        Guid creditCardId,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd,
        DateTimeOffset? statementDate = null,
        DateTimeOffset? dueDate = null,
        decimal? minimumPayment = null,
        CancellationToken cancellationToken = default)
        => closeCreditCardStatementHandler.Handle(new CloseCreditCardStatementCommand(creditCardId, periodStart, periodEnd, statementDate, dueDate, minimumPayment), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists credit card statements.")]
    public Task<IReadOnlyList<CreditCardStatementSummary>> ListCreditCardStatements(Guid? creditCardId = null, CancellationToken cancellationToken = default)
        => getCreditCardStatementsHandler.Handle(new GetCreditCardStatementsQuery(creditCardId), cancellationToken);
}
