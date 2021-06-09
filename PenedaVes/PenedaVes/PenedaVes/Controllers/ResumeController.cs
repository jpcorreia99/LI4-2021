using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PenedaVes.Data.Repository;
using PenedaVes.Models;
using PenedaVes.Services.Email;
using PenedaVes.Services.Phone;

namespace PenedaVes.Controllers
{   
    [Route("api/[action]")]
    [ApiController] //This attribute indicates that the controller responds to web API request
    public class ResumeController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public ResumeController(IRepository repository,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ISmsService smsService)
        {
            _repository = repository;
            _userManager = userManager;
            _emailService = emailService;
            _smsService = smsService;
        }

        public async Task SendResume()
        {
            List<ApplicationUser> users = await _userManager.Users.ToListAsync();
            DateTime lowerLimit = DateTime.Today;
            DateTime upperLimit = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            foreach (ApplicationUser user in users)
            {
                if (!user.ReceiveSummary) continue;
                List<Sighting> sightings = (await _repository.GetFollowedSightings(user, lowerLimit, upperLimit))
                    .OrderBy(s => s.CaptureMoment).ToList();
                
                string resume = ComposeResume(sightings);

                if (user.UseEmail)
                {   
                    Console.WriteLine("Sending email to: " + user.UserName);
                    //await _emailService.SendEmail(user.Email, "Resumo diário", resume);
                }

                if (user.UseCellphone)
                {
                    Console.WriteLine("Sending sms to: " + user.UserName);
                    //await _smsService.SendSms(user.PhoneNumber, resume);
                }

                Console.WriteLine(resume);
            }
        }

        private string ComposeResume(List<Sighting> sightings)
        {
            String resume = "Resumo dia: " +DateTime.Now.ToString("dd/MM")+ "\n\n";
            
            foreach (var sighting in sightings)
            {
                resume += sighting.CaptureMoment.ToString("HH:mm") +
                          "\n" + "Câmara: " + sighting.Camera.Name +
                          "\n" + sighting.Species.CommonName + " - " + sighting.Quantity + " Indivíduos." +
                          "\n--------------------------\n\n";
            }

            return resume;
        }
    }
}