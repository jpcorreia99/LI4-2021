using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PenedaVes.Models
{   
    public class Camera
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        [DisplayName("Nome")]
        public string Name { get; set; }
        
        [Required]
        public float Latitude { get; set; }
        [Required]
        public float Longitude { get; set; }
        
        [Required]
        [DisplayName("Zona Restrita?")]
        public bool RestrictedZone { get; set; }
        
        [Required]
        [DisplayName("Exclusivo a Admins?")]
        public bool RestrictedArea { get; set; }
        
        public List<FollowedCamera> FollowedCameras { get; set; } // many-to-many table between users and cameras

        public Camera(int id, string name, float latitude, float longitude, bool restrictedZone, bool restrictedArea)
        {
            Id = id;
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            RestrictedZone = restrictedZone;
            RestrictedArea = restrictedArea;
        }

        public Camera(string name, double latitude, double longitude, bool restrictedZone, bool restrictedArea)
        {
            Name = name;
            Latitude = (float) latitude;
            Longitude = (float) longitude;
            RestrictedZone = restrictedZone;
            RestrictedArea = restrictedArea;
        }

        public override string ToString()
        {
            return "Id: " + Id + "\n"
                   + "Name: " + Name;
        }
    }
}