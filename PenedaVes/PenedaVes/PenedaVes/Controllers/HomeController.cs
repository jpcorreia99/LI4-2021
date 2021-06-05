using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PenedaVes.Configuration;
using PenedaVes.Data;
using PenedaVes.Data.FileManager;
using PenedaVes.Models;
using PenedaVes.ViewModels;

namespace PenedaVes.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IOptions<BingSettings> _bingSettings;
        private readonly IFileManager _fileManager;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public HomeController(AppDbContext context,
            IOptions<BingSettings> bingSettings,
            IFileManager fileManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _bingSettings = bingSettings;
            _fileManager = fileManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            DateTime sevenDaysAgo = DateTime.Today.AddDays(-7);

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            
            List<Camera> followedCameras = await _context.FollowedCamera
                .Include(fc => fc.Camera)
                .Where(fc => fc.UserId.Equals(user.Id))
                .Select(fc => fc.Camera)
                .ToListAsync();
            
            List<Species> followedSpecies = await _context.FollowedSpecies
                .Include(fs => fs.Camera)
                .Where(fs => fs.UserId.Equals(user.Id))
                .Select(fs => fs.Camera)
                .ToListAsync();

            List<CameraInfo> camerasInfoList = (from camera in followedCameras
                let sightingCount = (
                    from sighting in _context.Sightings
                    where sighting.CameraId == camera.Id &&
                          sighting.CaptureMoment > sevenDaysAgo &&
                          followedCameras.Contains(sighting.Camera) && // select only sightings in the users camera preferences
                          followedSpecies.Contains(sighting.Species) // select only sightings in the users species preferences
                    select sighting).Count() 
                select new CameraInfo(camera.Name, camera.Latitude, camera.Longitude, camera.RestrictedZone,
                    Url.Action("Details", "Cameras", new {id = camera.Id}), sightingCount)).ToList();


            List<Sighting> sightingList = await (from sighting in _context.Sightings
                                        where sighting.CaptureMoment > sevenDaysAgo &&
                                              followedCameras.Contains(sighting.Camera) && // select only sightings in the users camera preferences
                                              followedSpecies.Contains(sighting.Species) // select only sightings in the users species preferences
                                        select sighting)
                                        .Include(s => s.Camera)
                                        .Include(s => s.Species)
                                        .OrderByDescending(x => x.CaptureMoment).ToListAsync(); 
            
            DashboardViewModel vm = new DashboardViewModel{
                Cameras = camerasInfoList,
                Sightings = sightingList,
                BingApiKey = _bingSettings.Value.ApiKey
            };

            return View(vm);
        }

        [HttpGet("/Image/{image}")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf('.') + 1); // after the '.'
            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}