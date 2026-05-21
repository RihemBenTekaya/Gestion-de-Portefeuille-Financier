namespace DashboardData.Models;

public class AssetPerformanceStat
{
    public int AssetId { get; set; }
    public string AssetName { get; set; } = "";
    public string AssetSymbol { get; set; } = "";
    public string AssetType { get; set; } = "";
    public decimal CurrentQuantity { get; set; }
    public decimal InvestedAmount { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal GainLoss { get; set; }
    public decimal PerformancePercent { get; set; }
}

public class AssetAllocationStat
{
    public string AssetType { get; set; } = "";
    public decimal CurrentValue { get; set; }
    public decimal AllocationPercent { get; set; }
}
