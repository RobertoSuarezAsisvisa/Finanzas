using FinanzasMCP.Application.Accounts.Commands;
using FinanzasMCP.Application.Accounts.Handlers;
using FinanzasMCP.Application.Accounts.Queries;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.CreditCards;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class AccountTools(
    CreateAccountHandler createAccountHandler,
    GetAccountsHandler getAccountsHandler,
    UpdateAccountHandler updateAccountHandler,
    DeleteAccountHandler deleteAccountHandler)
{
    [McpServerTool, System.ComponentModel.Description("Registers a new financial account.")]
    public Task<AccountSummary> CreateAccount(
        string name,
        AccountType accountType,
        string currency = "USD",
        AccountPurpose purpose = AccountPurpose.Spending,
        decimal balance = 0m,
        string? bankName = null,
        string? accountNumber = null,
        string? provider = null,
        string? cryptoSymbol = null,
        string? cryptoNetwork = null,
        decimal? cryptoQuantity = null,
        decimal? cryptoAvgBuyPriceUsd = null,
        string? creditCardIssuer = null,
        CreditCardBrand? creditCardBrand = null,
        string? creditCardProductName = null,
        string? creditCardLastFour = null,
        decimal? creditLimit = null,
        decimal? outstandingBalance = null,
        int? statementClosingDay = null,
        int? paymentDueDay = null,
        CreditCardPaymentMode? paymentMode = null,
        string? rewardsProgram = null,
        CreditCardStatementDelivery? statementDelivery = null,
        decimal? interestNominalAnnual = null,
        decimal? interestEffectiveAnnual = null,
        CancellationToken cancellationToken = default)
        => createAccountHandler.Handle(new CreateAccountCommand(name, accountType, currency, purpose, balance, bankName, accountNumber, provider, cryptoSymbol, cryptoNetwork, cryptoQuantity, cryptoAvgBuyPriceUsd, creditCardIssuer, creditCardBrand, creditCardProductName, creditCardLastFour, creditLimit, outstandingBalance, statementClosingDay, paymentDueDay, paymentMode, rewardsProgram, statementDelivery, interestNominalAnnual, interestEffectiveAnnual), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists the stored financial accounts.")]
    public Task<IReadOnlyList<AccountSummary>> ListAccounts(CancellationToken cancellationToken = default)
        => getAccountsHandler.Handle(new GetAccountsQuery(), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates an existing account.")]
    public Task<AccountSummary> UpdateAccount(
        Guid id,
        string name,
        AccountType accountType,
        string currency = "USD",
        AccountPurpose purpose = AccountPurpose.Spending,
        decimal balance = 0m,
        string? bankName = null,
        string? accountNumber = null,
        string? provider = null,
        string? cryptoSymbol = null,
        string? cryptoNetwork = null,
        decimal? cryptoQuantity = null,
        decimal? cryptoAvgBuyPriceUsd = null,
        string? creditCardIssuer = null,
        CreditCardBrand? creditCardBrand = null,
        string? creditCardProductName = null,
        string? creditCardLastFour = null,
        decimal? creditLimit = null,
        int? statementClosingDay = null,
        int? paymentDueDay = null,
        CreditCardPaymentMode? paymentMode = null,
        string? rewardsProgram = null,
        CreditCardStatementDelivery? statementDelivery = null,
        decimal? interestNominalAnnual = null,
        decimal? interestEffectiveAnnual = null,
        bool isActive = true,
        CancellationToken cancellationToken = default)
        => updateAccountHandler.Handle(new UpdateAccountCommand(id, name, accountType, currency, purpose, balance, bankName, accountNumber, provider, cryptoSymbol, cryptoNetwork, cryptoQuantity, cryptoAvgBuyPriceUsd, creditCardIssuer, creditCardBrand, creditCardProductName, creditCardLastFour, creditLimit, statementClosingDay, paymentDueDay, paymentMode, rewardsProgram, statementDelivery, interestNominalAnnual, interestEffectiveAnnual, isActive), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes an account.")]
    public Task DeleteAccount(Guid id, CancellationToken cancellationToken = default)
        => deleteAccountHandler.Handle(new DeleteAccountCommand(id), cancellationToken);
}
