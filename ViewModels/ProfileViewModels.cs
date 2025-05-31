using System.ComponentModel.DataAnnotations;

namespace UsersApp.ViewModels
{
    public class ProfileViewModel
    {
        public required string Name { get; set; }
        public required string Id { get; set; }
        public required string? Email { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [CustomValidation(typeof(DateValidator), nameof(DateValidator.ValidateNotFutureDate))]
        public DateTime? DOB { get; set; }
        public static class DateValidator
        {
        public static ValidationResult? ValidateNotFutureDate(DateTime? dob, ValidationContext context)
        {
            if (dob.HasValue && dob.Value.Date > DateTime.Today)
            {
                return new ValidationResult("Date of Birth cannot be in the future.");
            }
            return ValidationResult.Success;
        }
        }

        public string? Address { get; set; }
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]
        public float? Xth_Marks { get; set; }
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]

        public float? XIIth_Marks { get; set; }
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]

        public float? UG_Marks { get; set; }
        
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]
        public float? PG_Marks { get; set; }
        
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits and numeric")]
        public string? PhoneNumber { get; set; }
         public string? Role { get; set; }
   
}
}
