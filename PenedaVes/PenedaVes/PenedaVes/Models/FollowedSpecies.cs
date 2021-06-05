namespace PenedaVes.Models
{
    public class FollowedSpecies
    {
        public int Id { get; set; }
        public int SpeciesId { get; set; }
        public string UserId {get; set; }

        public virtual Species Camera { get; set; }
        public virtual ApplicationUser User {get; set; }
    }
}