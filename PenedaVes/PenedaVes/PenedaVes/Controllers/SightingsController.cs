using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PenedaVes.Data;
using PenedaVes.Models;
using PenedaVes.Services.Email;

namespace PenedaVes.Controllers
{
    [Route("api/[action]")]
    [ApiController] //This attribute indicates that the controller responds to web API request

    // When the [action] token isn't in the route template, the action name is excluded from the route. 
    // That is, the action's associated method name isn't used in the matching route.
    public class SightingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IEmailService _emailService;

        public SightingsController(AppDbContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: api/GetSpecies
        [HttpGet]
        public async Task<IActionResult> GetSpecies()
        {
            var query = from species in _context.Species
                select new {species.Id, species.CommonName};

            return Ok(await query.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetCameras()
        {
            var query = from camera in _context.Camera
                select new {camera.Id, camera.Name};

            return Ok(await query.ToListAsync());
        }
        
        // POST: api/ApiMovies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostSighting(Sighting sighting)
        {   
            var camera = await _context.Camera.FindAsync(sighting.CameraId);
            if (camera == null)
            {
                return NotFound("Camera does not exist");
            }
            
            var species = await _context.Species.FindAsync(sighting.SpeciesId);
            if (species == null)
            {
                return NotFound("Species does not exist");
            }
            
            _context.Sightings.Add(sighting);
            await _context.SaveChangesAsync();
            
            // Console.WriteLine(sighting.ToString());

            if (sighting.Species.CommonName.Equals("Humano") && sighting.Camera.RestrictedZone)
            {
                Console.WriteLine("Omg é um o mano no sítio proíbido!!");
                await _emailService.SendEmail("","Welcome", "Thank you for registering!");
            }
            return Ok(sighting);
        }
    }
}
