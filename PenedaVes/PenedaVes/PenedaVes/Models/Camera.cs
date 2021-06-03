using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PenedaVes.Models
{   //  BROOOOOOOO O DBsET
    public class Camera
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public float Latitude { get; set; }
        [Required]
        public float Longitude { get; set; }
        [Required]
        public bool RestrictedZone { get; set; }
        [Required]
        public bool RestrictedArea { get; set; }
    }
}