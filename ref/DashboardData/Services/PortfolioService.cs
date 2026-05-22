using DashboardData.Data;
using DashboardData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace DashboardData.Services;

public class PortfolioService : IPortfolioService
{
    private readonly AppDbContext _dbContext;
    private readonly AuthenticationStateProvider _authStateProvider;

    public PortfolioService(AppDbContext dbContext, AuthenticationStateProvider authStateProvider)
    {
        _dbContext = dbContext;
        _authStateProvider = authStateProvider;
    }

    private async Task<string> GetCurrentUserIdAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userId ?? "";
    }

    public async Task<List<Asset>> GetAssetsAsync()
    {
        var userId = await GetCurrentUserIdAsync();
        return await _dbContext.Assets
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.Type)
            .ThenBy(a => a.Symbol)
            .ToListAsync();
    }

    public async Task<List<Asset>> SearchAssetsAsync(string assetType, string searchText)
    {
        var userId = await GetCurrentUserIdAsync();
        IQueryable<Asset> query = _dbContext.Assets.Where(a => a.UserId == userId);

        if (!string.IsNullOrEmpty(assetType))
        {
            query = query.Where(a => a.Type == assetType);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(a => a.Name.Contains(searchText) || a.Symbol.Contains(searchText));
        }

        return await query
            .OrderBy(a => a.Type)
            .ThenBy(a => a.Symbol)
            .ToListAsync();
    }

    public async Task<Asset> GetAssetByIdAsync(int id)
    {
        var userId = await GetCurrentUserIdAsync();
        return await _dbContext.Assets.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
    }

    public async Task AddAssetAsync(Asset asset)
    {
        var userId = await GetCurrentUserIdAsync();
        asset.UserId = userId;
        asset.LastUpdate = DateTime.Now;
        _dbContext.Assets.Add(asset);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAssetAsync(Asset asset)
    {
        var userId = await GetCurrentUserIdAsync();
        var existingAsset = await _dbContext.Assets.FirstOrDefaultAsync(a => a.Id == asset.Id && a.UserId == userId);
        if (existingAsset != null)
        {
            existingAsset.Name = asset.Name;
            existingAsset.Symbol = asset.Symbol;
            existingAsset.Type = asset.Type;
            existingAsset.CurrentPrice = asset.CurrentPrice;
            existingAsset.Notes = asset.Notes;
            existingAsset.LastUpdate = DateTime.Now;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAssetAsync(int id)
    {
        var userId = await GetCurrentUserIdAsync();
        var asset = await _dbContext.Assets.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (asset != null)
        {
            _dbContext.Assets.Remove(asset);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<PortfolioTransaction>> GetTransactionsAsync()
    {
        var userId = await GetCurrentUserIdAsync();
        return await _dbContext.PortfolioTransactions
            .Include(t => t.Asset)
            .Where(t => t.Asset.UserId == userId)
            .OrderByDescending(t => t.OperationDate)
            .ToListAsync();
    }

    public async Task<List<PortfolioTransaction>> SearchTransactionsAsync(string assetType, string searchText)
    {
        var userId = await GetCurrentUserIdAsync();
        IQueryable<PortfolioTransaction> query = _dbContext.PortfolioTransactions
            .Include(t => t.Asset)
            .Where(t => t.Asset.UserId == userId);

        if (!string.IsNullOrEmpty(assetType))
        {
            query = query.Where(t => t.Asset.Type == assetType);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(t => t.Asset.Name.Contains(searchText) || t.Asset.Symbol.Contains(searchText));
        }

        return await query
            .OrderByDescending(t => t.OperationDate)
            .ToListAsync();
    }

    public async Task<PortfolioTransaction> GetTransactionByIdAsync(int id)
    {
        var userId = await GetCurrentUserIdAsync();
        return await _dbContext.PortfolioTransactions
            .Include(t => t.Asset)
            .FirstOrDefaultAsync(t => t.Id == id && t.Asset.UserId == userId);
    }

    public async Task AddTransactionAsync(PortfolioTransaction transaction)
    {
        _dbContext.PortfolioTransactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTransactionAsync(PortfolioTransaction transaction)
    {
        var userId = await GetCurrentUserIdAsync();
        var existingTransaction = await _dbContext.PortfolioTransactions
            .Include(t => t.Asset)
            .FirstOrDefaultAsync(t => t.Id == transaction.Id && t.Asset.UserId == userId);
        
        if (existingTransaction != null)
        {
            existingTransaction.AssetId = transaction.AssetId;
            existingTransaction.OperationType = transaction.OperationType;
            existingTransaction.Quantity = transaction.Quantity;
            existingTransaction.UnitPrice = transaction.UnitPrice;
            existingTransaction.Fees = transaction.Fees;
            existingTransaction.OperationDate = transaction.OperationDate;
            existingTransaction.Note = transaction.Note;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteTransactionAsync(int id)
    {
        var userId = await GetCurrentUserIdAsync();
        var transaction = await _dbContext.PortfolioTransactions
            .Include(t => t.Asset)
            .FirstOrDefaultAsync(t => t.Id == id && t.Asset.UserId == userId);
        
        if (transaction != null)
        {
            _dbContext.PortfolioTransactions.Remove(transaction);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<InvestmentBudget>> GetBudgetsAsync()
    {
        var userId = await GetCurrentUserIdAsync();
        return await _dbContext.InvestmentBudgets
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartDate)
            .ToListAsync();
    }

    public async Task<InvestmentBudget> GetBudgetByIdAsync(int id)
    {
        var userId = await GetCurrentUserIdAsync();
        return await _dbContext.InvestmentBudgets.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
    }

    public async Task AddBudgetAsync(InvestmentBudget budget)
    {
        var userId = await GetCurrentUserIdAsync();
        budget.UserId = userId;
        _dbContext.InvestmentBudgets.Add(budget);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateBudgetAsync(InvestmentBudget budget)
    {
        var userId = await GetCurrentUserIdAsync();
        var existingBudget = await _dbContext.InvestmentBudgets.FirstOrDefaultAsync(b => b.Id == budget.Id && b.UserId == userId);
        
        if (existingBudget != null)
        {
            existingBudget.Label = budget.Label;
            existingBudget.Amount = budget.Amount;
            existingBudget.StartDate = budget.StartDate;
            existingBudget.Notes = budget.Notes;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteBudgetAsync(int id)
    {
        var userId = await GetCurrentUserIdAsync();
        var budget = await _dbContext.InvestmentBudgets.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        
        if (budget != null)
        {
            _dbContext.InvestmentBudgets.Remove(budget);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<int> GetAssetCountAsync()
    {
        var userId = await GetCurrentUserIdAsync();
        return await _dbContext.Assets.CountAsync(a => a.UserId == userId);
    }

    public async Task<decimal> GetBudgetTotalAsync()
    {
        var userId = await GetCurrentUserIdAsync();
        if (!await _dbContext.InvestmentBudgets.AnyAsync(b => b.UserId == userId)) return 0;
        return await _dbContext.InvestmentBudgets.Where(b => b.UserId == userId).SumAsync(b => b.Amount);
    }

    public async Task<decimal> GetInvestedAmountAsync()
    {
        var stats = await GetPerformanceByAssetAsync();
        return stats.Sum(s => s.InvestedAmount);
    }

    public async Task<decimal> GetPortfolioValueAsync()
    {
        var stats = await GetPerformanceByAssetAsync();
        return stats.Sum(s => s.CurrentValue);
    }

    public async Task<decimal> GetGlobalGainLossAsync()
    {
        var stats = await GetPerformanceByAssetAsync();
        return stats.Sum(s => s.GainLoss);
    }

    public async Task<List<AssetPerformanceStat>> GetPerformanceByAssetAsync()
    {
        var userId = await GetCurrentUserIdAsync();
        var transactions = await _dbContext.PortfolioTransactions
            .Include(t => t.Asset)
            .Where(t => t.Asset.UserId == userId)
            .ToListAsync();

        return transactions
            .GroupBy(t => t.Asset)
            .Select(g =>
            {
                decimal quantity = g.Sum(t => t.OperationType == "Buy" ? t.Quantity : -t.Quantity);
                decimal invested = g.Sum(t => t.OperationType == "Buy"
                    ? (t.Quantity * t.UnitPrice) + t.Fees
                    : -((t.Quantity * t.UnitPrice) - t.Fees));
                decimal currentValue = quantity * g.Key.CurrentPrice;
                decimal gainLoss = currentValue - invested;

                return new AssetPerformanceStat
                {
                    AssetId = g.Key.Id,
                    AssetName = g.Key.Name,
                    AssetSymbol = g.Key.Symbol,
                    AssetType = g.Key.Type,
                    CurrentQuantity = quantity,
                    InvestedAmount = invested,
                    CurrentValue = currentValue,
                    GainLoss = gainLoss,
                    PerformancePercent = invested == 0 ? 0 : (gainLoss / invested) * 100
                };
            })
            .OrderByDescending(s => s.CurrentValue)
            .ToList();
    }

    public async Task<List<AssetAllocationStat>> GetAllocationByTypeAsync()
    {
        var performance = await GetPerformanceByAssetAsync();
        decimal totalValue = performance.Sum(s => s.CurrentValue);

        return performance
            .GroupBy(s => s.AssetType)
            .Select(g =>
            {
                decimal value = g.Sum(s => s.CurrentValue);
                return new AssetAllocationStat
                {
                    AssetType = g.Key,
                    CurrentValue = value,
                    AllocationPercent = totalValue == 0 ? 0 : (value / totalValue) * 100
                };
            })
            .OrderByDescending(s => s.CurrentValue)
            .ToList();
    }
}
