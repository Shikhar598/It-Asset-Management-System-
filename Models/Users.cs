using Microsoft.AspNetCore.Identity;

namespace Usersapp.models
{
    public class Users : IdentityUser 
    {
    public required string FullName { get; set; }
    public DateTime? DOB { get; set; }
    public string? Address { get; set; }
    public float? Xth_Marks { get; set; }
    public float? XIIth_Marks { get; set; }
    public float? UG_Marks { get; set; }
    public float? PG_Marks { get; set; }
    public string? Role { get; set; }
    }
}
