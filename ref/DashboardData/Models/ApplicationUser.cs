using Microsoft.AspNetCore.Identity;

namespace DashboardData.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = "";
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
}
