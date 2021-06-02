using System;
using System.ComponentModel.DataAnnotations;

namespace PenedaVes.Models
{
    public class Sighting
    {
        [Required] public int Id { get; set; }
        [Required] public int CameraId { get; set; }
        [Required] public int SpeciesId { get; set; }
        [Required] public DateTime CaptureMoment { get; set; } = DateTime.Now; 
        [Required] public int Quantity { get; set; }

        public virtual Camera Camera { get; set; }
        public virtual Species Species { get; set; }
        
        
        public override string ToString()
        {
            return "Id: " + Id + "\n" +
                   "Camera: " + CameraId + ",Species: " + SpeciesId + ", Qt: " + Quantity +
                   "Date: " + CaptureMoment;
        }
    }
}
