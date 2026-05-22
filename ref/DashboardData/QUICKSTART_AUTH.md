# Quick Start Guide - Authentication

## Step 1: Delete Old Database (Important!)

Since we added authentication, you need to recreate the database:

```powershell
Remove-Item app.db -ErrorAction SilentlyContinue
```

## Step 2: Apply Migrations

```powershell
dotnet ef database update
```

This will create:
- All Identity tables (AspNetUsers, AspNetRoles, etc.)
- Updated Assets and InvestmentBudgets tables with UserId
- A demo user account

## Step 3: Run the Application

```powershell
dotnet run
```

Or with specific URL:
```powershell
dotnet run --urls "http://localhost:5123"
```

## Step 4: Login

Open your browser to `http://localhost:5123`

You'll be redirected to the login page. Use the demo account:
- **Email**: demo@portfolio.com
- **Password**: Demo123

## Step 5: Test the Features

After logging in, you'll see:
- Your personalized dashboard
- Demo portfolio data (assets, transactions, budgets)
- Your email displayed in the navigation menu
- Logout option

## Create Your Own Account

1. Click "Register here" on the login page
2. Fill in:
   - Full Name
   - Email
   - Password (minimum 6 characters)
   - Confirm Password
3. Click "Create Account"
4. You'll be automatically logged in with an empty portfolio

## What Changed?

### User Experience
- ✅ Login required to access portfolio
- ✅ Each user has isolated data
- ✅ User email shown in navigation
- ✅ Easy logout

### Technical
- ✅ ASP.NET Core Identity integrated
- ✅ User authentication and authorization
- ✅ Password hashing and security
- ✅ User-specific data filtering
- ✅ Protected routes with [Authorize]

## Troubleshooting

### "Cannot open database file"
Delete `app.db` and run `dotnet ef database update` again.

### "Invalid email or password"
Make sure you're using:
- Email: demo@portfolio.com
- Password: Demo123 (case-sensitive)

### "Page not found" after login
Clear your browser cache and try again.

## Next Steps

- Create your own account
- Add your real assets
- Track your actual portfolio
- Invite others to use the app (each with their own account)

## Security Notes

- Passwords are hashed (never stored in plain text)
- Each user can only see their own data
- Sessions are managed securely by ASP.NET Core
- HTTPS is recommended for production use
