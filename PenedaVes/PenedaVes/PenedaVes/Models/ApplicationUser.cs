using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace PenedaVes.Models
{
    public class ApplicationUser : IdentityUser
    {
        [DisplayName("Receber Resumo?")]
        public bool ReceiveSummary { get; set; }
        
        [DisplayName("Usar Telemóvel?")]
        public bool UseCellphone { get; set; }
        
        [DisplayName("Usar email?")]
        public bool UseEmail { get; set; }

        public List<FollowedCamera> FollowedCameras { get; set; }
        public List<FollowedSpecies> FollowedSpecies { get; set; } 
    }
}