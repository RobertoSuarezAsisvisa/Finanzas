using FinanzasMCP.Application;
using FinanzasMCP.Infrastructure;
using FinanzasMCP.McpServer.Tools;
using ModelContextProtocol.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options => options.Stateless = true)
    .WithTools<AccountTools>()
    .WithTools<CryptoAccountTools>()
    .WithTools<CategoryTools>()
    .WithTools<TagTools>()
    .WithTools<TransactionTools>()
    .WithTools<BudgetTools>()
    .WithTools<CryptoLotTools>()
    .WithTools<AccountingPeriodTools>()
    .WithTools<SavingGoalTools>()
    .WithTools<PurchaseGoalTools>()
    .WithTools<DebtTools>()
    .WithTools<DebtPaymentTools>()
    .WithTools<RecurringRuleTools>()
    .WithTools<UserContextTools>()
    .WithTools<ReportTools>();

var app = builder.Build();
app.MapMcp("/mcp");
app.Run();
