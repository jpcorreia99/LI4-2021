using Microsoft.AspNetCore.Identity;

namespace PenedaVes.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool ReceiveSummary { get; set; }
        public bool UseCellphone { get; set; }
        public bool UseEmail { get; set; }
    }
}