using DashboardData.Components;
using DashboardData.Data;
using DashboardData.Models;
using DashboardData.Services;
using Microsoft.EntityFrameworkCore;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//===== Configure Entity Framework Core with SQLite ======

// Get the connection string from appsettings.json and configure the DbContext to use SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// This registers the AppDbContext with the dependency injection system, so it can be injected into components and other services that need to access the database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
//================================================================================


// Register application services with the scoped lifetime used in class for Blazor and EF Core.
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddRadzenComponents();

var app = builder.Build();

//===== Generate portfolio test data if the database is empty ======
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();

    if (!dbContext.Assets.Any())
    {
        Console.WriteLine("--- Generating portfolio demo data ---");

        var mainBudget = new InvestmentBudget
        {
            Label = "Main Investment Budget",
            Amount = 20000,
            StartDate = DateTime.Today,
            Notes = "Global budget used by the dashboard indicators."
        };

        var apple = new Asset { Name = "Apple Inc.", Symbol = "AAPL", Type = "Stock", CurrentPrice = 215.50m };
        var microsoft = new Asset { Name = "Microsoft", Symbol = "MSFT", Type = "Stock", CurrentPrice = 430.20m };
        var bitcoin = new Asset { Name = "Bitcoin", Symbol = "BTC", Type = "Crypto", CurrentPrice = 65000m };
        var ethereum = new Asset { Name = "Ethereum", Symbol = "ETH", Type = "Crypto", CurrentPrice = 3150m };
        var etf = new Asset { Name = "S&P 500 ETF", Symbol = "VOO", Type = "ETF", CurrentPrice = 505.40m };

        dbContext.InvestmentBudgets.Add(mainBudget);
        dbContext.Assets.AddRange(apple, microsoft, bitcoin, ethereum, etf);
        dbContext.SaveChanges();

        dbContext.PortfolioTransactions.AddRange(
            new PortfolioTransaction { AssetId = apple.Id, OperationType = "Buy", Quantity = 18, UnitPrice = 180m, Fees = 4m, OperationDate = DateTime.Today.AddDays(-70), Note = "Initial position" },
            new PortfolioTransaction { AssetId = microsoft.Id, OperationType = "Buy", Quantity = 9, UnitPrice = 390m, Fees = 4m, OperationDate = DateTime.Today.AddDays(-55), Note = "Software exposure" },
            new PortfolioTransaction { AssetId = bitcoin.Id, OperationType = "Buy", Quantity = 0.08m, UnitPrice = 52000m, Fees = 18m, OperationDate = DateTime.Today.AddDays(-45), Note = "Crypto allocation" },
            new PortfolioTransaction { AssetId = ethereum.Id, OperationType = "Buy", Quantity = 1.4m, UnitPrice = 2900m, Fees = 12m, OperationDate = DateTime.Today.AddDays(-30), Note = "Crypto diversification" },
            new PortfolioTransaction { AssetId = etf.Id, OperationType = "Buy", Quantity = 10, UnitPrice = 470m, Fees = 3m, OperationDate = DateTime.Today.AddDays(-25), Note = "Long term ETF" },
            new PortfolioTransaction { AssetId = apple.Id, OperationType = "Sell", Quantity = 4, UnitPrice = 205m, Fees = 2m, OperationDate = DateTime.Today.AddDays(-10), Note = "Partial profit taking" });

        dbContext.SaveChanges();
    }
}
//================================================================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
