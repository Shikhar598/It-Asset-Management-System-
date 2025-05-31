using System.ComponentModel.DataAnnotations;

namespace Usersapp.ViewModels

{ public class ChangePasswordViewModels
{
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmNewPassword",ErrorMessage ="Paassword does not match")]
        [Display(Name ="New Password")]
        public required string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Confirm New Password")]
        public required string ConfirmNewPassword { get; set; }
}
}
