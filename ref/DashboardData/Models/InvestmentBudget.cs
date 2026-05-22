using System.ComponentModel.DataAnnotations;

namespace DashboardData.Models;

public class InvestmentBudget
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = "";

    public ApplicationUser User { get; set; }

    [Required(ErrorMessage = "Budget label is required.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Budget label must be between 2 and 60 characters.")]
    public string Label { get; set; } = "Main Budget";

    [Range(typeof(decimal), "1", "999999999", ConvertValueInInvariantCulture = true, ParseLimitsInInvariantCulture = true, ErrorMessage = "Budget amount must be greater than 0.")]
    public decimal Amount { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Now;

    [StringLength(250)]
    public string Notes { get; set; } = "";
}
