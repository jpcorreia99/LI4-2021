using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PenedaVes.Models
{
    public class Species
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
        [Required]
        [StringLength(100)]
        public string Image { get; set; }
    }
}