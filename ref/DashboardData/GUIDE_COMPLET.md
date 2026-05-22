# 📚 Guide Complet du Projet - Green Portfolio

## 🎯 Vue d'ensemble du projet

**Green Portfolio** est une application web de gestion de portefeuille financier développée avec **Blazor Server** (.NET 10). Elle permet aux utilisateurs de suivre leurs investissements (actions, crypto, ETF), d'enregistrer des transactions et de gérer des budgets d'investissement.

### Technologies utilisées
- **Framework**: ASP.NET Core 10 avec Blazor Server
- **Base de données**: SQLite avec Entity Framework Core
- **Authentification**: ASP.NET Core Identity
- **UI**: Bootstrap 5 + Bootstrap Icons
- **Graphiques**: Radzen Blazor Components

---

## 📁 Architecture du projet

```
DashboardData/
├── Components/              # Composants Blazor
│   ├── Layout/             # Layouts et navigation
│   │   ├── MainLayout.razor       # Layout principal avec sidebar
│   │   ├── NavMenu.razor          # Menu de navigation
│   │   └── ReconnectModal.razor   # Modal de reconnexion WebSocket
│   ├── Pages/              # Pages de l'application
│   │   ├── Login.razor            # Page de connexion
│   │   ├── MyDashboard.razor      # Dashboard principal
│   │   ├── EditAsset.razor        # Créer/Modifier un asset
│   │   ├── EditTransaction.razor  # Créer/Modifier une transaction
│   │   ├── EditBudget.razor       # Créer/Modifier un budget
│   │   ├── Budgets.razor          # Liste des budgets
│   │   ├── Error.razor            # Page d'erreur
│   │   └── NotFound.razor         # Page 404
│   ├── UI/                 # Composants UI réutilisables
│   │   ├── AssetTable.razor       # Tableau des assets
│   │   ├── BudgetTable.razor      # Tableau des budgets
│   │   ├── KpiCard.razor          # Carte KPI
│   │   └── TransactionTable.razor # Tableau des transactions
│   ├── App.razor           # Composant racine
│   ├── Routes.razor        # Configuration du routage
│   └── _Imports.razor      # Imports globaux
├── Data/                   # Contexte de base de données
│   └── AppDbContext.cs            # DbContext EF Core
├── Migrations/             # Migrations EF Core
│   ├── 20260513162127_InitialCreate.cs
│   └── 20260521203635_AddIdentityAndUserRelations.cs
├── Models/                 # Modèles de données
│   ├── ApplicationUser.cs         # Utilisateur (Identity)
│   ├── Asset.cs                   # Asset financier
│   ├── InvestmentBudget.cs        # Budget d'investissement
│   ├── PortfolioTransaction.cs    # Transaction
│   └── PortfolioStats.cs          # Statistiques (DTO)
├── Services/               # Services métier
│   └── PortfolioService.cs        # Service principal
├── wwwroot/                # Fichiers statiques
├── Program.cs              # Point d'entrée et configuration
├── appsettings.json        # Configuration
└── app.db                  # Base de données SQLite

```

---

## 🗄️ Modèles de données

### 1. **ApplicationUser** (Utilisateur)
```csharp
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }           // Nom complet
    public DateTime RegistrationDate { get; set; }  // Date d'inscription
    
    // Relations
    public ICollection<Asset> Assets { get; set; }
    public ICollection<InvestmentBudget> Budgets { get; set; }
}
```

### 2. **Asset** (Actif financier)
```csharp
public class Asset
{
    public int Id { get; set; }
    public string UserId { get; set; }              // Propriétaire
    public string Name { get; set; }                // Ex: "Apple Inc."
    public string Symbol { get; set; }              // Ex: "AAPL"
    public string Type { get; set; }                // Stock, Crypto, ETF, Bond, Cash
    public decimal CurrentPrice { get; set; }       // Prix actuel
    public DateTime LastUpdate { get; set; }        // Dernière mise à jour
    public string Notes { get; set; }               // Notes optionnelles
    
    // Relations
    public ApplicationUser User { get; set; }
    public ICollection<PortfolioTransaction> Transactions { get; set; }
}
```

### 3. **PortfolioTransaction** (Transaction)
```csharp
public class PortfolioTransaction
{
    public int Id { get; set; }
    public int AssetId { get; set; }                // Asset concerné
    public string OperationType { get; set; }       // "Buy" ou "Sell"
    public decimal Quantity { get; set; }           // Quantité
    public decimal UnitPrice { get; set; }          // Prix unitaire
    public decimal Fees { get; set; }               // Frais
    public DateTime OperationDate { get; set; }     // Date de l'opération
    public string Note { get; set; }                // Note optionnelle
    
    // Relations
    public Asset Asset { get; set; }
}
```

### 4. **InvestmentBudget** (Budget)
```csharp
public class InvestmentBudget
{
    public int Id { get; set; }
    public string UserId { get; set; }              // Propriétaire
    public string Label { get; set; }               // Ex: "Budget principal"
    public decimal Amount { get; set; }             // Montant
    public DateTime StartDate { get; set; }         // Date de début
    public string Notes { get; set; }               // Notes optionnelles
    
    // Relations
    public ApplicationUser User { get; set; }
}
```

### 5. **PortfolioStats** (Statistiques - DTO)
```csharp
public class PortfolioStats
{
    public decimal TotalInvested { get; set; }      // Total investi
    public decimal CurrentValue { get; set; }       // Valeur actuelle
    public decimal ProfitLoss { get; set; }         // Gain/Perte
    public decimal ProfitLossPercent { get; set; }  // Gain/Perte en %
    public int AssetCount { get; set; }             // Nombre d'assets
    public decimal BudgetTotal { get; set; }        // Budget total
}
```

---

## 🔐 Système d'authentification

### Architecture
L'authentification utilise **ASP.NET Core Identity** avec une approche spéciale pour Blazor Server:

1. **Problème**: Blazor Server utilise WebSocket (SignalR), impossible de définir des cookies après connexion
2. **Solution**: Utiliser des formulaires HTML classiques avec `data-enhance="false"` pour contourner le WebSocket

### Endpoints d'authentification (Program.cs)

```csharp
// Login - POST /api/auth/login
app.MapPost("/api/auth/login", async (
    SignInManager<ApplicationUser> signInManager,
    [FromForm] string email, 
    [FromForm] string password) =>
{
    var result = await signInManager.PasswordSignInAsync(
        email, password, isPersistent: false, lockoutOnFailure: false);
    
    if (result.Succeeded) 
        return Results.Redirect("/dashboard");
    
    return Results.Redirect("/login?error=Identifiants+incorrects");
}).DisableAntiforgery();

// Logout - POST /api/auth/logout
app.MapPost("/api/auth/logout", async (
    SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/login");
}).DisableAntiforgery();
```

### Page de login (Login.razor)

```razor
<form method="post" action="/api/auth/login" data-enhance="false">
    <input type="text" name="email" required />
    <input type="password" name="password" required />
    <button type="submit">Se connecter</button>
</form>
```

**Points clés:**
- `method="post"` : Utilise HTTP POST standard
- `action="/api/auth/login"` : Pointe vers l'endpoint
- `data-enhance="false"` : **CRUCIAL** - Désactive le WebSocket de Blazor
- `name="email"` et `name="password"` : Correspondent aux paramètres `[FromForm]`

### Protection des pages

```razor
@page "/dashboard"
@attribute [Authorize]  // ← Protège la page
```

### Affichage conditionnel (MainLayout.razor)

```razor
<AuthorizeView>
    <Authorized>
        <!-- Contenu pour utilisateurs connectés -->
        <p>Bonjour @context.User.Identity?.Name</p>
    </Authorized>
    <NotAuthorized>
        <!-- Contenu pour utilisateurs non connectés -->
        <a href="/login">Se connecter</a>
    </NotAuthorized>
</AuthorizeView>
```

---

## 🔧 Service métier (PortfolioService)

Le `PortfolioService` gère toute la logique métier et filtre automatiquement les données par utilisateur.

### Récupération de l'utilisateur connecté

```csharp
private async Task<string?> GetCurrentUserIdAsync()
{
    var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
    var user = authState.User;
    return user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
}
```

### Exemples de méthodes

```csharp
// Récupérer les assets de l'utilisateur connecté
public async Task<List<Asset>> GetAssetsAsync()
{
    var userId = await GetCurrentUserIdAsync();
    return await _context.Assets
        .Where(a => a.UserId == userId)
        .OrderBy(a => a.Type)
        .ThenBy(a => a.Symbol)
        .ToListAsync();
}

// Calculer les statistiques du portefeuille
public async Task<PortfolioStats> GetPortfolioStatsAsync()
{
    var userId = await GetCurrentUserIdAsync();
    
    var transactions = await _context.PortfolioTransactions
        .Include(t => t.Asset)
        .Where(t => t.Asset.UserId == userId)
        .ToListAsync();
    
    // Calculs...
    return new PortfolioStats { ... };
}
```

---

## 🎨 Pages principales

### 1. **MyDashboard.razor** - Dashboard principal

**Fonctionnalités:**
- Affiche 4 KPI (Total investi, Valeur actuelle, Gain/Perte, Budget)
- Graphique en barres des assets par type
- Tableau des assets avec actions (Éditer/Supprimer)
- Tableau des dernières transactions

**Code clé:**
```razor
@attribute [Authorize]
@inject IPortfolioService PortfolioService

protected override async Task OnInitializedAsync()
{
    Stats = await PortfolioService.GetPortfolioStatsAsync();
    Assets = await PortfolioService.GetAssetsAsync();
    RecentTransactions = await PortfolioService.GetRecentTransactionsAsync(10);
}
```

### 2. **EditAsset.razor** - Créer/Modifier un asset

**Routes:**
- `/assets/new` - Créer un nouvel asset
- `/assets/edit/{Id}` - Modifier un asset existant

**Assignation automatique du UserId:**
```csharp
protected override async Task OnInitializedAsync()
{
    var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
    var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    if (CurrentAsset == null)
    {
        CurrentAsset = new Asset 
        { 
            UserId = userId ?? "",  // ← Assigné dès le chargement
            Type = "Stock", 
            CurrentPrice = 1 
        };
    }
}
```

### 3. **EditTransaction.razor** - Enregistrer une transaction

**Fonctionnalités:**
- Sélection de l'asset dans une liste déroulante
- Type d'opération: Buy ou Sell
- Calcul automatique du total (Quantité × Prix + Frais)

**Validation:**
- L'asset doit appartenir à l'utilisateur connecté
- Les transactions sont liées à l'asset, pas directement à l'utilisateur

### 4. **Budgets.razor** - Gestion des budgets

**Fonctionnalités:**
- Liste tous les budgets de l'utilisateur
- Affiche le total des budgets
- Permet de créer, modifier et supprimer des budgets

---

## 🔄 Flux de données

### Exemple: Création d'un asset

```
1. Utilisateur clique sur "New Asset"
   ↓
2. EditAsset.razor charge
   - OnInitializedAsync() récupère le UserId
   - Crée un nouvel Asset avec UserId pré-rempli
   ↓
3. Utilisateur remplit le formulaire
   ↓
4. Utilisateur clique "Save Asset"
   - HandleValidSubmit() est appelé
   - PortfolioService.AddAssetAsync() sauvegarde en DB
   ↓
5. Redirection vers /dashboard
   ↓
6. Dashboard recharge et affiche le nouvel asset
```

### Exemple: Calcul des statistiques

```
1. Dashboard appelle GetPortfolioStatsAsync()
   ↓
2. Service récupère le UserId de l'utilisateur connecté
   ↓
3. Requête SQL filtrée par UserId:
   - Récupère toutes les transactions de l'utilisateur
   - Calcule: Total investi, Valeur actuelle, Gain/Perte
   ↓
4. Retourne PortfolioStats
   ↓
5. Dashboard affiche les KPI
```

---

## 🗃️ Base de données (SQLite)

### Configuration (Program.cs)

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
```

### Chaîne de connexion (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  }
}
```

### Migrations

```bash
# Créer une migration
dotnet ef migrations add NomDeLaMigration

# Appliquer les migrations
dotnet ef database update
```

**Note:** Les migrations sont appliquées automatiquement au démarrage dans `Program.cs`:
```csharp
dbContext.Database.Migrate();
```

---

## 🚀 Démarrage du projet

### Prérequis
- .NET 10 SDK
- Visual Studio 2022 ou VS Code

### Commandes

```bash
# Restaurer les packages
dotnet restore

# Compiler
dotnet build

# Lancer l'application
dotnet run

# Accéder à l'application
http://localhost:5191
```

### Compte de démonstration

```
Email: demo@portfolio.com
Mot de passe: Demo123
```

---

## 📊 Composants UI réutilisables

### KpiCard.razor
Affiche une carte KPI avec titre, valeur, sous-titre et icône.

```razor
<KpiCard 
    Title="Total Invested" 
    Value="$15,234.50" 
    Subtitle="Across all assets" 
    BackgroundColorClass="bg-primary" 
    IconClass="bi-wallet2" />
```

### AssetTable.razor
Tableau des assets avec actions (Éditer/Supprimer).

```razor
<AssetTable 
    Assets="@Assets" 
    OnDeleteClicked="DeleteAsset" />
```

### TransactionTable.razor
Tableau des transactions avec détails (Asset, Type, Quantité, Prix, Total).

```razor
<TransactionTable 
    Transactions="@RecentTransactions" 
    OnDeleteClicked="DeleteTransaction" />
```

---

## 🔍 Points techniques importants

### 1. **Blazor Server vs Blazor WebAssembly**
- **Blazor Server**: Exécution côté serveur, communication via WebSocket (SignalR)
- Avantage: Accès direct à la base de données, pas de téléchargement de DLL
- Inconvénient: Nécessite une connexion permanente au serveur

### 2. **Rendermode InteractiveServer**
```razor
@rendermode InteractiveServer
```
Active l'interactivité côté serveur pour les composants (événements, binding).

### 3. **EditForm et validation**
```razor
<EditForm Model="CurrentAsset" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary class="text-danger" />
    <InputText @bind-Value="CurrentAsset.Name" />
    <ValidationMessage For="@(() => CurrentAsset.Name)" />
</EditForm>
```

### 4. **Injection de dépendances**
```razor
@inject IPortfolioService PortfolioService
@inject NavigationManager NavigationManager
@inject AuthenticationStateProvider AuthenticationStateProvider
```

### 5. **Gestion des erreurs**
- Page Error.razor pour les erreurs non gérées
- Page NotFound.razor pour les routes inexistantes
- ReconnectModal.razor pour les déconnexions WebSocket

---

## 🎯 Bonnes pratiques appliquées

1. ✅ **Séparation des responsabilités**: Services, Modèles, Composants UI
2. ✅ **Validation des données**: DataAnnotations sur les modèles
3. ✅ **Sécurité**: Authentification, autorisation, filtrage par utilisateur
4. ✅ **Réutilisabilité**: Composants UI génériques (KpiCard, Tables)
5. ✅ **Migrations**: Gestion des changements de schéma de base de données
6. ✅ **Configuration**: appsettings.json pour les paramètres
7. ✅ **Seeding**: Création automatique d'un utilisateur de démo

---

## 📝 Améliorations possibles

1. **Inscription d'utilisateurs**: Ajouter une page de registration
2. **Récupération de mot de passe**: Email de réinitialisation
3. **API externe**: Récupérer les prix en temps réel (Yahoo Finance, Alpha Vantage)
4. **Graphiques avancés**: Évolution du portefeuille dans le temps
5. **Export**: Exporter les données en CSV/Excel
6. **Notifications**: Alertes de prix, rappels
7. **Multi-devises**: Support de plusieurs devises
8. **Rôles**: Admin, User, Viewer
9. **Tests**: Tests unitaires et d'intégration
10. **Docker**: Conteneurisation de l'application

---

## 🆘 Dépannage

### Erreur: "Headers are read-only"
**Cause**: Tentative de définir des cookies depuis un contexte WebSocket Blazor.
**Solution**: Utiliser des formulaires HTML avec `data-enhance="false"`.

### Erreur: "The UserId field is required"
**Cause**: UserId non assigné lors de la création d'un asset/budget.
**Solution**: Assigner le UserId dans `OnInitializedAsync()`.

### Port déjà utilisé
**Cause**: Une instance du serveur tourne déjà.
**Solution**: 
```bash
# Trouver le processus
netstat -ano | findstr :5191

# Tuer le processus
Stop-Process -Id <PID> -Force
```

---

## 📚 Ressources

- [Documentation Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Radzen Blazor Components](https://blazor.radzen.com/)
- [Bootstrap 5](https://getbootstrap.com/)

---

**Créé le**: 22 mai 2026  
**Version**: 1.0  
**Auteur**: Green Portfolio Team
