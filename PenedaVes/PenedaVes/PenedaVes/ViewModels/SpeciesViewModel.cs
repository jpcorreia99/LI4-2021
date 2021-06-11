using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PenedaVes.ViewModels
{
    public class SpeciesViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string CommonName { get; set; }
        [Required]
        [StringLength(100)]
        public string ScientificName { get; set; }
        public string Description { get; set; }
        [Required]
        public bool IsPredatory { get; set; }

        public string CurrentImage { get; set; }
        public IFormFile Image { get; set; }
    }
}