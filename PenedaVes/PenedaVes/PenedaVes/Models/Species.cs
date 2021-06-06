using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PenedaVes.Models
{
    public class Species
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        [DisplayName("Nome Comum")]
        public string CommonName { get; set; }
        
        [Required]
        [StringLength(100)]
        [DisplayName("Nome científico")]
        public string ScientificName { get; set; }
        
        [DisplayName("Descrição")]
        public string Description { get; set; }
        
        [Required]
        [DisplayName("Predatória?")]
        public bool IsPredatory { get; set; }
        
        [DisplayName("Link Imagem")]
        [Required]
        [StringLength(100)]
        public string Image { get; set; }

        public List<FollowedSpecies> FollowedSpecies { get; set; } // many-to-many table between users and species
    }
}