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

        [Display(Name = "Data de início")]
        [DataType(DataType.Date)]
        public DateTime BeginningDate { get; set; } = DateTime.Today.AddDays(-7);

        [Display(Name = "Data de fim")]
        [DataType(DataType.Date)]
        public DateTime EndingDate { get; set; } = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        
        public PieChart PieChart { get; set; }
        public BarChart BarChart { get; set; }
        
        
    }
    
    public class PieChart
    {
        public List<string> Labels { get; set; }
        public List<ChildPieChart> Datasets { get; set;}
        public PieChart()
        {
            Labels = new List<string>();
            Datasets = new List<ChildPieChart>();
        }
    }

    public class ChildPieChart
    {
        public List<string> BackgroundColor { get; set; } 
        public List<int> Data { get; set; }

        public ChildPieChart()
        {
            BackgroundColor = new List<string>();
            Data = new List<int>();
        }
    }
}