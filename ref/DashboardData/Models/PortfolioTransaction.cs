using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardData.Models;

public class PortfolioTransaction
{
    [Key]
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select an asset.")]
    public int AssetId { get; set; }

    public Asset Asset { get; set; }

    [Required(ErrorMessage = "Operation type is required.")]
    [StringLength(10)]
    public string OperationType { get; set; } = "Buy";

    [Range(typeof(decimal), "0.0001", "999999999", ConvertValueInInvariantCulture = true, ParseLimitsInInvariantCulture = true, ErrorMessage = "Quantity must be greater than 0.")]
    public decimal Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "999999999", ConvertValueInInvariantCulture = true, ParseLimitsInInvariantCulture = true, ErrorMessage = "Unit price must be greater than 0.")]
    public decimal UnitPrice { get; set; }

    [Range(typeof(decimal), "0", "999999999", ConvertValueInInvariantCulture = true, ParseLimitsInInvariantCulture = true, ErrorMessage = "Fees cannot be negative.")]
    public decimal Fees { get; set; }

    public DateTime OperationDate { get; set; } = DateTime.Now;

    [StringLength(250)]
    public string Note { get; set; } = "";

    [NotMapped]
    public decimal GrossAmount => Quantity * UnitPrice;
}
