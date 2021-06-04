using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PenedaVes.Data;
using PenedaVes.Models;
using PenedaVes.Services.Email;
using PenedaVes.Services.Phone;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

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
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public SightingsController(AppDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ISmsService smsService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _smsService = smsService;
        }

        // GET: api/GetSpecies
        [HttpGet]
        public async Task<IActionResult> GetSpecies()
        {
            var query = from species in _context.Species
                select new {species.Id, species.CommonName, species.IsPredatory};

            return Ok(await query.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetCameras()
        {
            var query = from camera in _context.Camera
                select new {camera.Id, camera.Name, camera.RestrictedZone};

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
            
            sighting.CaptureMoment=DateTime.Now;

            _context.Sightings.Add(sighting);
            await _context.SaveChangesAsync();
            
            // if it's a dangerous situation, will notify admins by sms/email
            await HandleDangerousSituation(sighting);

            return Ok(sighting);
        }

        /**
         * Checks if the given sighting corresponds to a dangerous situation and,
         * if so, send an email to the park's admins.
         */
        private async Task HandleDangerousSituation(Sighting sighting)
        {
            string message;
            switch (sighting.Species.CommonName)
            {
                case "Humano" when sighting.Camera.RestrictedZone:
                   // await AlertAdmins("Humano em zona restrita:\nCâmara: "+ sighting.Camera.Name);
                   message = "Humano em zona restrita.\nCâmara: "+ sighting.Camera.Name;
                   await AlertAdmins(message);
                    break;
                case "Humano":
                {
                    DateTime fiveMinutesAgo = DateTime.Now.AddMinutes(-5);
                    
                    bool predatorySpeciesSeen = (from filteredSightings in _context.Sightings
                            where filteredSightings.CameraId == sighting.CameraId // sightings in that camera
                                  &&  filteredSightings.CaptureMoment > fiveMinutesAgo // in the last 5 minutes
                                  && filteredSightings.Species.IsPredatory // where there are predatory species
                            select filteredSightings.Species) // pick the species
                        .Any(); // check if list isn't empty;

                    if (predatorySpeciesSeen)
                    {
                        message = "Humano em contacto com espécies predatórias.\nCâmara: "+ sighting.Camera.Name;
                        await AlertAdmins(message);
                    }
                    break;
                }
                default:
                {
                    if (sighting.Species.IsPredatory)
                    {
                        DateTime fiveMinutesAgo = DateTime.Now.AddMinutes(-5);
                    
                        bool HumansSeen = (from filteredSightings in _context.Sightings
                                where filteredSightings.CameraId == sighting.CameraId // sightings in that camera
                                      &&  filteredSightings.CaptureMoment > fiveMinutesAgo // in the last 5 minutes
                                      && filteredSightings.Species.CommonName.Equals("Humano") // where there are predatory species
                                select filteredSightings) // pick the sightings that contain humans
                            .Any(); // check if list isn't empty;

                        if (HumansSeen)
                        {
                            message = "Espécie predatória em contacto com área frequentada por humanos." +
                                      "\nCâmara: " + sighting.Camera.Name + 
                                      "\nEspécie: " + sighting.Species.CommonName;
                            await AlertAdmins(message);
                        }
                    }
                    break;
                }
            }
        }
        private async Task AlertAdmins(string message)
        {
            Console.WriteLine("Message to send: "+message);
            
            var adminUsers = await _userManager.GetUsersInRoleAsync("admin");
            foreach (ApplicationUser user in adminUsers)
            {

                if (user.UseEmail)
                {
                    Console.WriteLine("Sending email to: " + user.UserName);
                    //await _emailService.SendEmail(user.Email, "Notificação de perigo!", message);
                }
                else if (user.UseCellphone)
                {
                    Console.WriteLine("Sending message to " + user.UserName + ", Phone number: " + user.PhoneNumber);
                    //await _smsService.SendSms(user.PhoneNumber, message);
                }
            }
        }
        
    }
}
