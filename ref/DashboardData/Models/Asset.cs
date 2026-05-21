using System.ComponentModel.DataAnnotations;

namespace DashboardData.Models;

public class Asset
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Asset name is required.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Asset name must be between 2 and 80 characters.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Symbol is required.")]
    [StringLength(12, MinimumLength = 1, ErrorMessage = "Symbol must be between 1 and 12 characters.")]
    public string Symbol { get; set; } = "";

    [Required(ErrorMessage = "Asset type is required.")]
    [StringLength(30)]
    public string Type { get; set; } = "Stock";

    [Range(typeof(decimal), "0.01", "999999999", ConvertValueInInvariantCulture = true, ParseLimitsInInvariantCulture = true, ErrorMessage = "Current price must be greater than 0.")]
    public decimal CurrentPrice { get; set; }

    public DateTime LastUpdate { get; set; } = DateTime.Now;

    [StringLength(250)]
    public string Notes { get; set; } = "";

    public ICollection<PortfolioTransaction> Transactions { get; set; } = new List<PortfolioTransaction>();
}
