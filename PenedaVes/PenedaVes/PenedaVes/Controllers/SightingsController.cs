using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private IEmailService _emailService;

        public SightingsController(AppDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
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

            await HandleDangerousSituation(sighting);

            return Ok(sighting);
        }

        /**
         * Checks if the given sighting corresponds to a dangerous situation and,
         * if so, send an email to the park's admins.
         */
        private async Task HandleDangerousSituation(Sighting sighting)
        {
            switch (sighting.Species.CommonName)
            {
                case "Humano" when sighting.Camera.RestrictedZone:
                   // await AlertAdmins("Humano em zona restrita:\nCâmara: "+ sighting.Camera.Name);
                   Console.WriteLine("Humano em zona restrita:\nCâmara: "+ sighting.Camera.Name);
                    break;
                case "Humano":
                {
                    bool predatorySpeciesSeen = (from filteredSightings in _context.Sightings
                            where filteredSightings.CameraId == sighting.CameraId // sightings in that camera
                                  && sighting.CaptureMoment.Subtract(
                                      filteredSightings.CaptureMoment).TotalMinutes < 5 // in the last 5 minutes
                                  && sighting.Species.IsPredatory // where there are predatory species
                            select filteredSightings.Species) // pick the species
                        .Any(); // check if list isn't empty;

                    if (predatorySpeciesSeen)
                    {
                        Console.Write("Big poopoo");
                    }
                    else
                    {
                        Console.WriteLine("No poopoo");
                    }

                    break;
                }
                default:
                {
                    if (sighting.Species.IsPredatory)
                    {
                        Console.WriteLine("---");
                    }

                    break;
                }
            }
        }
        private async Task AlertAdmins(string message)
        {
            Console.WriteLine("Listing Admins");
            var adminUsers = await _userManager.GetUsersInRoleAsync("admin");
            foreach (ApplicationUser user in adminUsers)
            {
                Console.WriteLine("Sending email to: "+ user.UserName);
                await _emailService.SendEmail(user.Email, "Notificação de perigo!", message);
            }

            Console.WriteLine("Completed");
        }
    }
}
