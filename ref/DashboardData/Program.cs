using DashboardData.Components;
using DashboardData.Data;
using DashboardData.Models;
using DashboardData.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//===== Configure Entity Framework Core with SQLite ======
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
//================================================================================

//===== Configure ASP.NET Core Identity ======
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddCascadingAuthenticationState();

// Configure cookie authentication to redirect to login page
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/login";
    options.AccessDeniedPath = "/login";
});
//================================================================================

// Register application services
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddRadzenComponents();

var app = builder.Build();

//===== Seeding: Create demo user and data ======
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // Apply migrations automatically
    dbContext.Database.Migrate();

    // Create demo user if doesn't exist
    if (await userManager.FindByEmailAsync("demo@portfolio.com") == null)
    {
        Console.WriteLine("--- Creating demo user ---");
        var demoUser = new ApplicationUser
        {
            UserName = "demo@portfolio.com",
            Email = "demo@portfolio.com",
            FullName = "Demo User",
            RegistrationDate = DateTime.Now
        };

        var result = await userManager.CreateAsync(demoUser, "Demo123");
        if (result.Succeeded)
        {
            Console.WriteLine("Demo user created: demo@portfolio.com / Demo123");

            // Add demo data
            var mainBudget = new InvestmentBudget
            {
                UserId = demoUser.Id,
                Label = "Main Investment Budget",
                Amount = 20000,
                StartDate = DateTime.Today,
                Notes = "Global budget used by the dashboard indicators."
            };

            var apple = new Asset { UserId = demoUser.Id, Name = "Apple Inc.", Symbol = "AAPL", Type = "Stock", CurrentPrice = 215.50m };
            var microsoft = new Asset { UserId = demoUser.Id, Name = "Microsoft", Symbol = "MSFT", Type = "Stock", CurrentPrice = 430.20m };
            var bitcoin = new Asset { UserId = demoUser.Id, Name = "Bitcoin", Symbol = "BTC", Type = "Crypto", CurrentPrice = 65000m };
            var ethereum = new Asset { UserId = demoUser.Id, Name = "Ethereum", Symbol = "ETH", Type = "Crypto", CurrentPrice = 3150m };
            var etf = new Asset { UserId = demoUser.Id, Name = "S&P 500 ETF", Symbol = "VOO", Type = "ETF", CurrentPrice = 505.40m };

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
}
//================================================================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// --- ENDPOINTS D'AUTHENTIFICATION (Hors WebSocket) ---
app.MapPost("/api/auth/login", async (
    [FromServices] SignInManager<ApplicationUser> signInManager,
    [FromForm] string email, 
    [FromForm] string password) =>
{
    var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
    
    if (result.Succeeded) return Results.Redirect("/dashboard");
    
    return Results.Redirect("/login?error=Identifiants+incorrects");
}).DisableAntiforgery(); 

app.MapPost("/api/auth/logout", async ([FromServices] SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/login");
}).DisableAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
