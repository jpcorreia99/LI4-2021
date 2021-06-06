using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PenedaVes.Models;

namespace PenedaVes.ViewModels
{
    public class SpeciesDetailsViewModel
    {
        public Species Species;
        
        public List<Sighting> CapturedSightings;

        [Display(Name = "Beginning Date")]
        [DataType(DataType.Date)]
        public DateTime BeginningDate { get; set; } = DateTime.Today.AddDays(-7);

        [Display(Name = "Ending Date")]
        [DataType(DataType.Date)]
        public DateTime EndingDate { get; set; } = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
    }
}