using DashboardData.Data;
using DashboardData.Models;
using Microsoft.EntityFrameworkCore;

namespace DashboardData.Services;

public class PortfolioService : IPortfolioService
{
    private readonly AppDbContext _dbContext;

    public PortfolioService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Asset>> GetAssetsAsync()
    {
        return await _dbContext.Assets
            .OrderBy(a => a.Type)
            .ThenBy(a => a.Symbol)
            .ToListAsync();
    }

    public async Task<List<Asset>> SearchAssetsAsync(string assetType, string searchText)
    {
        IQueryable<Asset> query = _dbContext.Assets.AsQueryable();

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
        return await _dbContext.Assets.FindAsync(id);
    }

    public async Task AddAssetAsync(Asset asset)
    {
        asset.LastUpdate = DateTime.Now;
        _dbContext.Assets.Add(asset);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAssetAsync(Asset asset)
    {
        asset.LastUpdate = DateTime.Now;
        _dbContext.Assets.Update(asset);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAssetAsync(int id)
    {
        var asset = await _dbContext.Assets.FindAsync(id);
        if (asset != null)
        {
            _dbContext.Assets.Remove(asset);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<PortfolioTransaction>> GetTransactionsAsync()
    {
        return await _dbContext.PortfolioTransactions
            .Include(t => t.Asset)
            .OrderByDescending(t => t.OperationDate)
            .ToListAsync();
    }

    public async Task<List<PortfolioTransaction>> SearchTransactionsAsync(string assetType, string searchText)
    {
        IQueryable<PortfolioTransaction> query = _dbContext.PortfolioTransactions
            .Include(t => t.Asset)
            .AsQueryable();

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
        return await _dbContext.PortfolioTransactions.FindAsync(id);
    }

    public async Task AddTransactionAsync(PortfolioTransaction transaction)
    {
        _dbContext.PortfolioTransactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTransactionAsync(PortfolioTransaction transaction)
    {
        _dbContext.PortfolioTransactions.Update(transaction);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteTransactionAsync(int id)
    {
        var transaction = await _dbContext.PortfolioTransactions.FindAsync(id);
        if (transaction != null)
        {
            _dbContext.PortfolioTransactions.Remove(transaction);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<InvestmentBudget>> GetBudgetsAsync()
    {
        return await _dbContext.InvestmentBudgets
            .OrderByDescending(b => b.StartDate)
            .ToListAsync();
    }

    public async Task<InvestmentBudget> GetBudgetByIdAsync(int id)
    {
        return await _dbContext.InvestmentBudgets.FindAsync(id);
    }

    public async Task AddBudgetAsync(InvestmentBudget budget)
    {
        _dbContext.InvestmentBudgets.Add(budget);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateBudgetAsync(InvestmentBudget budget)
    {
        _dbContext.InvestmentBudgets.Update(budget);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteBudgetAsync(int id)
    {
        var budget = await _dbContext.InvestmentBudgets.FindAsync(id);
        if (budget != null)
        {
            _dbContext.InvestmentBudgets.Remove(budget);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<int> GetAssetCountAsync()
    {
        return await _dbContext.Assets.CountAsync();
    }

    public async Task<decimal> GetBudgetTotalAsync()
    {
        if (!await _dbContext.InvestmentBudgets.AnyAsync()) return 0;
        return await _dbContext.InvestmentBudgets.SumAsync(b => b.Amount);
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
        var transactions = await _dbContext.PortfolioTransactions
            .Include(t => t.Asset)
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
