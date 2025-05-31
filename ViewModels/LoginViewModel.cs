using System.ComponentModel.DataAnnotations;
namespace Usersapp.ViewModels
{
    public class loginViewModel{
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Passeord is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberMe{ get; set; }

    }
}