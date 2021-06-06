using PenedaVes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PenedaVes.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {

        }
        public DbSet<Camera> Camera { get; set; }
        public DbSet<Species> Species  { get; set; }
        public DbSet<Sighting> Sightings  { get; set; }
        public DbSet<FollowedCamera> FollowedCamera  { get; set; }
        public DbSet<FollowedSpecies> FollowedSpecies  { get; set; }
    }
}