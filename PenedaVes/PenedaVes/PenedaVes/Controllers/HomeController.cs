using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IOptions<BingSettings> _bingSettings;
        private IFileManager _fileManager;
        
        public HomeController(AppDbContext context,
            IOptions<BingSettings> bingSettings,
            IFileManager fileManager)
        {
            _context = context;
            _bingSettings = bingSettings;
            _fileManager = fileManager;
        }

        public IActionResult Index()
        {
            DateTime sevenDaysAgo = DateTime.Today.AddDays(-7);
            
            List<CameraInfo> camerasInfoList = (from camera in _context.Camera.ToList()
                let sightingCount = (
                    from sightings in _context.Sightings
                    where sightings.CameraId == camera.Id &&
                          sightings.CaptureMoment > sevenDaysAgo
                    select sightings).Count() 
                select new CameraInfo(camera.Name, camera.Latitude, camera.Longitude, camera.RestrictedZone,
                    Url.Action("Details", "Cameras", new {id = camera.Id}), sightingCount)).ToList();


            List<Sighting> sightingList = (from sightings in _context.Sightings
                                        where sightings.CaptureMoment > sevenDaysAgo
                                        select sightings)
                                        .Include(s => s.Camera)
                                        .Include(s => s.Species)
                                        .OrderByDescending(x => x.CaptureMoment).ToList(); 
            
            DashboardViewModel vm = new DashboardViewModel{
                Cameras = camerasInfoList,
                Sightings = sightingList,
                BingApiKey = _bingSettings.Value.ApiKey
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
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