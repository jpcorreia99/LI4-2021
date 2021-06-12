using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PenedaVes.Models;

namespace PenedaVes.ViewModels
{
    public class CameraDetailsViewModel
    {
        public Camera Camera;
        
        public List<Sighting> CapturedSightings;

        [Display(Name = "Data de início")]
        [DataType(DataType.Date)]
        public DateTime BeginningDate { get; set; } = DateTime.Today.AddDays(-7);

        [Display(Name = "Data de fim")]
        [DataType(DataType.Date)]
        public DateTime EndingDate { get; set; } = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        
        // Chart for number of sightings captured in each day
        public BarChart BarChart { get; set; }
    }
}