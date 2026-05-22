# Authentication Implementation

## Overview

This project now includes user authentication using ASP.NET Core Identity. Each user has their own isolated portfolio data.

## Features Added

### 1. User Registration and Login
- **Register**: `/account/register` - Create a new account with email and password
- **Login**: `/account/login` - Sign in to access your portfolio
- **Logout**: `/account/logout` - Sign out of your account

### 2. User Isolation
All portfolio data is now user-specific:
- **Assets**: Each user can only see and manage their own assets
- **Transactions**: Transactions are linked to user's assets
- **Budgets**: Each user has their own investment budgets

### 3. Protected Pages
The dashboard and all portfolio management pages require authentication. Unauthenticated users are redirected to the login page.

## Database Changes

### New Tables (Identity)
- `AspNetUsers` - User accounts
- `AspNetRoles` - User roles
- `AspNetUserRoles` - User-role relationships
- `AspNetUserClaims` - User claims
- `AspNetUserLogins` - External login providers
- `AspNetUserTokens` - Authentication tokens
- `AspNetRoleClaims` - Role claims

### Modified Tables
- `Assets` - Added `UserId` foreign key
- `InvestmentBudgets` - Added `UserId` foreign key
- `PortfolioTransactions` - Linked through Asset's UserId

## Demo Account

A demo account is automatically created on first run:
- **Email**: demo@portfolio.com
- **Password**: Demo123

## Password Requirements

Current settings (can be modified in Program.cs):
- Minimum length: 6 characters
- No special character requirements
- No uppercase/lowercase requirements
- No digit requirements

## How to Run

1. **Delete old database** (if exists):
   ```powershell
   Remove-Item app.db -ErrorAction SilentlyContinue
   ```

2. **Apply migrations**:
   ```powershell
   dotnet ef database update
   ```

3. **Run the application**:
   ```powershell
   dotnet run
   ```

4. **Access the application**:
   - Navigate to `http://localhost:5123`
   - You'll be redirected to the login page
   - Use the demo account or register a new one

## Code Changes Summary

### Models
- **ApplicationUser.cs** - Custom user model extending IdentityUser
- **Asset.cs** - Added UserId property and User navigation
- **InvestmentBudget.cs** - Added UserId property and User navigation

### Data
- **AppDbContext.cs** - Now inherits from IdentityDbContext<ApplicationUser>

### Services
- **PortfolioService.cs** - Updated all methods to filter by current user

### Pages
- **Login.razor** - User login page
- **Register.razor** - User registration page
- **Logout.razor** - Logout handler
- **MyDashboard.razor** - Added [Authorize] attribute

### Configuration
- **Program.cs** - Added Identity services and authentication middleware
- **NavMenu.razor** - Added login/logout links with AuthorizeView
- **_Imports.razor** - Added authorization namespaces

## Security Features

1. **Password Hashing**: Passwords are hashed using Identity's default hasher
2. **User Isolation**: Users can only access their own data
3. **Authentication State**: Managed through Blazor's AuthenticationStateProvider
4. **Protected Routes**: Pages require authentication via [Authorize] attribute

## Customization

### Change Password Requirements
Edit `Program.cs`:
```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
```

### Add More User Properties
Edit `Models/ApplicationUser.cs`:
```csharp
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = "";
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    // Add more properties here
}
```

## Testing

1. **Register a new user**:
   - Go to `/account/register`
   - Fill in the form
   - Submit

2. **Login**:
   - Go to `/account/login`
   - Enter credentials
   - Submit

3. **Verify isolation**:
   - Create assets/transactions with User A
   - Logout
   - Login as User B
   - Verify User B cannot see User A's data

## Troubleshooting

### Migration Issues
If you encounter migration errors:
```powershell
# Remove all migrations
Remove-Item -Recurse Migrations

# Create fresh migration
dotnet ef migrations add InitialCreateWithIdentity

# Apply migration
dotnet ef database update
```

### Authentication Not Working
1. Ensure `app.UseAuthentication()` comes before `app.UseAuthorization()` in Program.cs
2. Verify `AddCascadingAuthenticationState()` is registered
3. Check that pages have `@attribute [Authorize]`

## Next Steps

Potential enhancements:
1. Email confirmation
2. Password reset functionality
3. Two-factor authentication
4. Social login (Google, Microsoft, etc.)
5. User profile management page
6. Role-based authorization (Admin, User, etc.)
