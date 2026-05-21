using DashboardData.Models;

namespace DashboardData.Services;

public interface IPortfolioService
{
    Task<List<Asset>> GetAssetsAsync();
    Task<List<Asset>> SearchAssetsAsync(string assetType, string searchText);
    Task<Asset> GetAssetByIdAsync(int id);
    Task AddAssetAsync(Asset asset);
    Task UpdateAssetAsync(Asset asset);
    Task DeleteAssetAsync(int id);

    Task<List<PortfolioTransaction>> GetTransactionsAsync();
    Task<List<PortfolioTransaction>> SearchTransactionsAsync(string assetType, string searchText);
    Task<PortfolioTransaction> GetTransactionByIdAsync(int id);
    Task AddTransactionAsync(PortfolioTransaction transaction);
    Task UpdateTransactionAsync(PortfolioTransaction transaction);
    Task DeleteTransactionAsync(int id);

    Task<List<InvestmentBudget>> GetBudgetsAsync();
    Task<InvestmentBudget> GetBudgetByIdAsync(int id);
    Task AddBudgetAsync(InvestmentBudget budget);
    Task UpdateBudgetAsync(InvestmentBudget budget);
    Task DeleteBudgetAsync(int id);

    Task<int> GetAssetCountAsync();
    Task<decimal> GetBudgetTotalAsync();
    Task<decimal> GetInvestedAmountAsync();
    Task<decimal> GetPortfolioValueAsync();
    Task<decimal> GetGlobalGainLossAsync();
    Task<List<AssetPerformanceStat>> GetPerformanceByAssetAsync();
    Task<List<AssetAllocationStat>> GetAllocationByTypeAsync();
}
