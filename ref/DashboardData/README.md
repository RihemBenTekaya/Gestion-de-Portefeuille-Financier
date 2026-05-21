# Green Portfolio

Green Portfolio is a Blazor Web App built with .NET 10, Entity Framework Core, SQLite, Bootstrap and Radzen. It manages a simple investment portfolio with assets, buy/sell transactions, investment budgets, operation history and analytical dashboard indicators.

The project follows the same techniques used in class and in the reference labs:

- Blazor Web App with Interactive Server rendering.
- Layered architecture: UI, Services, Data, Models.
- Entity Framework Core Code-First with SQLite and migrations.
- CRUD pages built with `EditForm`, `DataAnnotationsValidator` and `ValidationMessage`.
- Dependency Injection with scoped services.
- Async EF Core queries using `Task`, `await`, `ToListAsync`, `CountAsync`, `SumAsync`.
- LINQ filtering, grouping, ordering and aggregation.
- Bootstrap layout and Bootstrap Icons.
- Radzen charts for dashboard visualization.

No external API, JavaScript framework, advanced state library, custom ORM, authentication framework or unstudied architecture is used.

## Project Goal

The objective is to build a simple web application for managing an investment portfolio.

Functional requirements covered:

- Asset management.
- Buy and sell transaction entry.
- Investment budget management.
- Operation history.
- Dynamic search and filtering.
- Analytical dashboard.

Analytical indicators covered:

- Current portfolio value.
- Global gain/loss.
- Performance by asset.
- Portfolio allocation by asset type.
- Budget usage percentage.

## Tech Stack

| Area | Technology |
| --- | --- |
| Web UI | Blazor Web App |
| Render mode | Interactive Server |
| Backend | ASP.NET Core / .NET 10 |
| Database | SQLite |
| ORM | Entity Framework Core |
| Charts | Radzen.Blazor |
| Styling | Bootstrap, Bootstrap Icons, small custom CSS |
| Validation | Data Annotations |
| Architecture | UI / Services / Data / Models |

## Required SDK

This project is configured for:

```powershell
C:\dotnet-sdk-10.0.202-win-x64\dotnet.exe
```

Use this SDK to avoid version problems.

## How To Run

Open a terminal in:

```powershell
C:\Users\KIRA\Desktop\hiba\ref\DashboardData
```

Run the application:

```powershell
$env:PATH="C:\dotnet-sdk-10.0.202-win-x64;$env:PATH"
& "C:\dotnet-sdk-10.0.202-win-x64\dotnet.exe" run
```

If you want to force the same URL used during testing:

```powershell
$env:PATH="C:\dotnet-sdk-10.0.202-win-x64;$env:PATH"
& "C:\dotnet-sdk-10.0.202-win-x64\dotnet.exe" run --urls "http://localhost:5123"
```

Then open:

```text
http://localhost:5123
```

## Build Command

```powershell
& "C:\dotnet-sdk-10.0.202-win-x64\dotnet.exe" build
```

Expected result:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

## Database Commands

The database is SQLite and stored in:

```text
app.db
```

List migrations:

```powershell
$env:PATH="C:\dotnet-sdk-10.0.202-win-x64;$env:PATH"
& "C:\dotnet-sdk-10.0.202-win-x64\dotnet.exe" ef migrations list --project "DashboardData.csproj" --startup-project "DashboardData.csproj"
```

Apply migrations:

```powershell
$env:PATH="C:\dotnet-sdk-10.0.202-win-x64;$env:PATH"
& "C:\dotnet-sdk-10.0.202-win-x64\dotnet.exe" ef database update --project "DashboardData.csproj" --startup-project "DashboardData.csproj"
```

Current migration:

```text
20260513162127_InitialCreate
```

## Folder Structure

```text
DashboardData/
  Components/
    Layout/
      MainLayout.razor
      NavMenu.razor
    Pages/
      MyDashboard.razor
      EditAsset.razor
      EditTransaction.razor
      Budgets.razor
      EditBudget.razor
      Error.razor
      NotFound.razor
    UI/
      KpiCard.razor
      AssetTable.razor
      TransactionTable.razor
      BudgetTable.razor
  Data/
    AppDbContext.cs
  Models/
    Asset.cs
    PortfolioTransaction.cs
    InvestmentBudget.cs
    PortfolioStats.cs
  Services/
    IPortfolioService.cs
    PortfolioService.cs
  Migrations/
    20260513162127_InitialCreate.cs
    AppDbContextModelSnapshot.cs
  wwwroot/
    app.css
  Program.cs
  appsettings.json
  app.db
```

## Architecture

The project uses a simple layered architecture.

| Layer | Files | Role |
| --- | --- | --- |
| UI | `Components/Pages`, `Components/UI` | Displays pages, forms, tables and charts. |
| Services | `Services/IPortfolioService.cs`, `Services/PortfolioService.cs` | Contains business logic and database queries. |
| Data | `Data/AppDbContext.cs` | Connects EF Core to SQLite and defines database tables. |
| Models | `Models/*.cs` | Defines entities, validation rules and dashboard DTOs. |

The UI never talks directly to `AppDbContext`. It injects `IPortfolioService`, and the service handles all EF Core work.

Example from a page:

```csharp
@inject IPortfolioService PortfolioService
```

Example from dependency injection in `Program.cs`:

```csharp
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
```

## Database Model

### Asset

Represents an investment asset, such as a stock, crypto currency, ETF, bond or cash item.

Main fields:

- `Id`: primary key.
- `Name`: asset name.
- `Symbol`: short symbol like AAPL, BTC, ETH.
- `Type`: Stock, Crypto, ETF, Bond or Cash.
- `CurrentPrice`: current market price entered by the user.
- `LastUpdate`: last asset update date.
- `Notes`: optional text.
- `Transactions`: one asset can have many transactions.

Validation examples:

```csharp
[Required]
[StringLength(80, MinimumLength = 2)]
public string Name { get; set; } = "";
```

### PortfolioTransaction

Represents a buy or sell operation.

Main fields:

- `Id`: primary key.
- `AssetId`: foreign key to `Asset`.
- `Asset`: navigation property.
- `OperationType`: Buy or Sell.
- `Quantity`: number of units.
- `UnitPrice`: price per unit.
- `Fees`: transaction fees.
- `OperationDate`: operation date.
- `Note`: optional note.
- `GrossAmount`: calculated property, not mapped to the database.

Relationship:

```text
Asset 1 ---- many PortfolioTransaction
```

### InvestmentBudget

Represents an investment budget line.

Main fields:

- `Id`: primary key.
- `Label`: budget label.
- `Amount`: budget amount.
- `StartDate`: start date.
- `Notes`: optional note.

### PortfolioStats

Contains DTO classes used only for dashboard analytics:

- `AssetPerformanceStat`
- `AssetAllocationStat`

These classes are not database tables. They are built from LINQ queries in the service.

## EF Core Context

`AppDbContext` defines the database tables:

```csharp
public DbSet<Asset> Assets { get; set; }
public DbSet<PortfolioTransaction> PortfolioTransactions { get; set; }
public DbSet<InvestmentBudget> InvestmentBudgets { get; set; }
```

The connection string is in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=app.db"
}
```

`Program.cs` registers EF Core with SQLite:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
```

## Initial Demo Data

On startup, `Program.cs` checks if the `Assets` table is empty.

If it is empty, it creates demo records:

- Apple Inc.
- Microsoft.
- Bitcoin.
- Ethereum.
- S&P 500 ETF.
- Main Investment Budget.
- Several buy/sell transactions.

This makes the dashboard useful immediately after running the project.

## Pages And Routes

| Route | Page | Purpose |
| --- | --- | --- |
| `/` | `MyDashboard.razor` | Main dashboard. |
| `/dashboard` | `MyDashboard.razor` | Main dashboard. |
| `/assets/new` | `EditAsset.razor` | Create a new asset. |
| `/assets/edit/{Id:int}` | `EditAsset.razor` | Edit an existing asset. |
| `/transactions/new` | `EditTransaction.razor` | Create a new buy/sell transaction. |
| `/transactions/edit/{Id:int}` | `EditTransaction.razor` | Edit an existing transaction. |
| `/budgets` | `Budgets.razor` | List budgets. |
| `/budgets/new` | `EditBudget.razor` | Create a budget. |
| `/budgets/edit/{Id:int}` | `EditBudget.razor` | Edit a budget. |

## Dashboard Features

The dashboard shows four KPI cards:

- Portfolio Value.
- Gain / Loss.
- Net Invested.
- Assets.

It also shows:

- Budget usage progress bar.
- Performance by asset column chart.
- Allocation by type donut chart.
- Assets table.
- Transaction history table.
- Search bar.
- Asset type filter.
- Toggle between asset table and transaction history.

## Analytics Logic

Analytics are calculated in `PortfolioService.cs`.

### Portfolio Value

For each asset:

```text
Current quantity * Current price
```

Current quantity is calculated from transactions:

```text
Buy quantity - Sell quantity
```

### Net Invested

For buys:

```text
(Quantity * UnitPrice) + Fees
```

For sells:

```text
-((Quantity * UnitPrice) - Fees)
```

### Gain / Loss

```text
Current value - Net invested
```

### Performance Percent

```text
(GainLoss / InvestedAmount) * 100
```

If invested amount is zero, performance is shown as zero to avoid division by zero.

### Allocation By Type

Assets are grouped by `Type` using LINQ `GroupBy`.

For each type:

```text
Type value / Total portfolio value * 100
```

## CRUD Workflows

### Add Asset

1. Open `/assets/new`.
2. Enter asset name, symbol, type and current price.
3. Click `Save Asset`.
4. The asset is saved in SQLite.
5. The app redirects to `/dashboard`.

### Edit Asset

1. Open `/dashboard`.
2. Click `Edit` on an asset row.
3. Modify values.
4. Click `Save Asset`.
5. The row is updated and dashboard indicators refresh.

### Add Transaction

1. Open `/transactions/new`.
2. Select an asset.
3. Select Buy or Sell.
4. Enter quantity, unit price, fees and date.
5. Click `Save Transaction`.
6. The transaction is saved and the dashboard recalculates analytics.

### Edit Transaction

1. Switch dashboard table to `History`.
2. Click `Edit` on a transaction row.
3. Modify values.
4. Click `Save Transaction`.
5. The transaction is updated.

### Add Budget

1. Open `/budgets/new`.
2. Enter label, amount, start date and notes.
3. Click `Save Budget`.
4. The budget is saved.

### Edit Budget

1. Open `/budgets`.
2. Click `Edit` on a budget row.
3. Modify values.
4. Click `Save Budget`.
5. The budget total updates.

### Delete Records

Tables contain `Delete` buttons.

- Deleting an asset removes the asset.
- Related transactions are removed by the database relationship.
- Deleting a transaction recalculates analytics.
- Deleting a budget recalculates the budget total.

## Form Handling Details

All Blazor forms use `EditForm`.

Each form has a unique `FormName` to avoid POST mapping errors:

```razor
<EditForm Model="CurrentAsset" OnValidSubmit="HandleValidSubmit" FormName="AssetEditForm">
```

The project uses these form names:

- `AssetEditForm`
- `TransactionEditForm`
- `BudgetEditForm`

Each form model uses `[SupplyParameterFromForm]` so Blazor can bind posted form values correctly:

```csharp
[SupplyParameterFromForm]
private Asset CurrentAsset { get; set; }
```

Edit forms also include hidden IDs so update workflows keep the correct primary key:

```html
<input type="hidden" name="CurrentAsset.Id" value="@CurrentAsset.Id" />
```

## Validation Details

Validation uses Data Annotations in the models.

Examples:

- `[Required]`
- `[StringLength]`
- `[Range]`

Decimal ranges are culture-safe.

This is important on systems using French culture, where commas are decimal separators. Without invariant parsing, values like `0.01` in `[Range]` can crash validation.

Example fix used in the project:

```csharp
[Range(typeof(decimal), "0.01", "999999999",
    ConvertValueInInvariantCulture = true,
    ParseLimitsInInvariantCulture = true,
    ErrorMessage = "Current price must be greater than 0.")]
public decimal CurrentPrice { get; set; }
```

## UI Components

### KpiCard

Reusable Bootstrap card for dashboard indicators.

Parameters:

- `Title`
- `Value`
- `Subtitle`
- `BackgroundColorClass`
- `IconClass`

### AssetTable

Displays asset rows with:

- Name and symbol.
- Type.
- Current price.
- Current quantity.
- Current value.
- Gain/loss.
- Edit and delete buttons.

### TransactionTable

Displays transaction history with:

- Date.
- Asset.
- Operation type.
- Quantity.
- Unit price.
- Fees.
- Total.
- Edit and delete buttons.

### BudgetTable

Displays budgets with:

- Label.
- Amount.
- Start date.
- Notes.
- Edit and delete buttons.

## Service Methods

`IPortfolioService` defines the contract used by the UI.

Asset methods:

- `GetAssetsAsync()`
- `SearchAssetsAsync(string assetType, string searchText)`
- `GetAssetByIdAsync(int id)`
- `AddAssetAsync(Asset asset)`
- `UpdateAssetAsync(Asset asset)`
- `DeleteAssetAsync(int id)`

Transaction methods:

- `GetTransactionsAsync()`
- `SearchTransactionsAsync(string assetType, string searchText)`
- `GetTransactionByIdAsync(int id)`
- `AddTransactionAsync(PortfolioTransaction transaction)`
- `UpdateTransactionAsync(PortfolioTransaction transaction)`
- `DeleteTransactionAsync(int id)`

Budget methods:

- `GetBudgetsAsync()`
- `GetBudgetByIdAsync(int id)`
- `AddBudgetAsync(InvestmentBudget budget)`
- `UpdateBudgetAsync(InvestmentBudget budget)`
- `DeleteBudgetAsync(int id)`

Analytics methods:

- `GetAssetCountAsync()`
- `GetBudgetTotalAsync()`
- `GetInvestedAmountAsync()`
- `GetPortfolioValueAsync()`
- `GetGlobalGainLossAsync()`
- `GetPerformanceByAssetAsync()`
- `GetAllocationByTypeAsync()`

## Search And Filtering

The dashboard has a search bar and type filter.

Search works on:

- Asset name.
- Asset symbol.

Type filter works on:

- Stock.
- Crypto.
- ETF.
- Bond.
- Cash.

Filtering is done in the service using `IQueryable`, like in class:

```csharp
IQueryable<Asset> query = _dbContext.Assets.AsQueryable();

if (!string.IsNullOrEmpty(assetType))
{
    query = query.Where(a => a.Type == assetType);
}

if (!string.IsNullOrWhiteSpace(searchText))
{
    query = query.Where(a => a.Name.Contains(searchText) || a.Symbol.Contains(searchText));
}
```

## Charts

Radzen is used for dashboard charts.

Performance chart:

```razor
<RadzenColumnSeries Data="@PerformanceStats"
                    CategoryProperty="AssetSymbol"
                    ValueProperty="GainLoss" />
```

Allocation chart:

```razor
<RadzenDonutSeries Data="@AllocationStats"
                   CategoryProperty="AssetType"
                   ValueProperty="CurrentValue" />
```

Clicking the allocation chart filters the dashboard table by asset type.

## Styling

The UI follows the same simple Bootstrap vibe as the reference project.

Design choices:

- Sidebar layout.
- Bootstrap cards.
- `card shadow-sm` panels.
- `table table-striped table-hover` tables.
- `table-dark` headers.
- Green theme using Bootstrap `btn-success`, `text-success`, `bg-success`.
- Minimal custom CSS in `wwwroot/app.css`.

## Tested Workflows

The project was tested with real page loads and real form POST submissions.

Verified routes:

- `/`
- `/dashboard`
- `/assets/new`
- `/assets/edit/1`
- `/assets/edit/999999`
- `/transactions/new`
- `/transactions/edit/1`
- `/transactions/edit/999999`
- `/budgets`
- `/budgets/new`
- `/budgets/edit/1`
- `/budgets/edit/999999`

Verified POST workflows:

- Create asset.
- Edit asset.
- Create transaction.
- Edit transaction.
- Create budget.
- Edit budget.
- Invalid decimal validation without crashing.

## Troubleshooting

### FormName POST Error

If you see:

```text
The POST request does not specify which form is being submitted.
```

Check that every `EditForm` has a unique `FormName`.

Correct example:

```razor
<EditForm Model="CurrentAsset" OnValidSubmit="HandleValidSubmit" FormName="AssetEditForm">
```

### Decimal Validation Crash

If you see an error like:

```text
0.01 is not a valid value for Decimal
```

Use invariant culture in decimal `Range` attributes:

```csharp
ConvertValueInInvariantCulture = true,
ParseLimitsInInvariantCulture = true
```

### App EXE Locked During Build

If build fails because `DashboardData.exe` is locked, stop the running app first.

PowerShell command:

```powershell
$connections = Get-NetTCPConnection -LocalPort 5123 -State Listen -ErrorAction SilentlyContinue
if ($connections) {
    $connections | Select-Object -ExpandProperty OwningProcess | Sort-Object -Unique | ForEach-Object {
        Stop-Process -Id $_ -Force
    }
}
```

Then rebuild:

```powershell
& "C:\dotnet-sdk-10.0.202-win-x64\dotnet.exe" build
```

### HTTPS Redirect Warning

During local HTTP testing, this warning can appear:

```text
Failed to determine the https port for redirect.
```

The app still runs on the HTTP URL. It is not blocking for the class demo.

## Evaluation Mapping

| Requirement | Where it is implemented |
| --- | --- |
| Blazor Web Application | `Components/Pages`, `Components/Layout`, `Program.cs` |
| Layered architecture | `UI`, `Services`, `Data`, `Models` folders |
| EF Core Code-First | `AppDbContext`, `Migrations`, `app.db` |
| CRUD | Asset, Transaction and Budget pages |
| Search and filters | Dashboard search bar and type filter |
| Dashboard indicators | KPI cards and analytics service methods |
| Charts | Radzen column chart and donut chart |
| Data Annotations | Model validation attributes |
| Async | All service methods use async EF Core queries |
| Relational database | SQLite with EF Core tables and foreign key |

## Notes For Presentation

Suggested demo order:

1. Open `/dashboard` and explain the KPI cards.
2. Show the performance chart and allocation chart.
3. Use search and type filter.
4. Add a new asset.
5. Add a buy transaction for that asset.
6. Return to dashboard and show indicators updated.
7. Open `/budgets` and add or edit a budget.
8. Explain the architecture folders.
9. Show `PortfolioService.cs` for LINQ and async EF Core.
10. Show `AppDbContext.cs` and the migration for Code-First.

## Important Scope Note

This project intentionally stays inside the studied class techniques. It does not use external APIs for live market prices. Current prices are entered manually, which keeps the project easy to explain and defend during presentation.
