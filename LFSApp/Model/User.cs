using System.ComponentModel.DataAnnotations;

namespace LFSApp.Model
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Firstname must be in uppercase, lowercase.")]
        [Display(Name = "Firstname")]
        public string Firstname { get; set; } = string.Empty;


        [Required(ErrorMessage = "Lastname must be in uppercase, lowercase.")]
        [Display(Name = "Lastname")]
        public string Lastname { get; set; } = string.Empty;

        
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password must be at least 6 characters long and include an uppercase letter, lowercase letter, number, and special character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
       
       
    }
}
