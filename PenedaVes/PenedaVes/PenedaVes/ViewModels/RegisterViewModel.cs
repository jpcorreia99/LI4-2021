using System.ComponentModel.DataAnnotations;

namespace PenedaVes.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public bool ReceiveSummary { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public bool IsAdmin { get; set; }
    }
}