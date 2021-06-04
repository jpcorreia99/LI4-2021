using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PenedaVes.Configuration;
using PenedaVes.Data;
using PenedaVes.Models;
using PenedaVes.ViewModels;

namespace PenedaVes.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IOptions<BingSettings> _bingSettings;
        
        public HomeController(AppDbContext context,
            IOptions<BingSettings> bingSettings)
        {
            _context = context;
            _bingSettings = bingSettings;
        }

        public IActionResult Index()
        {
            DateTime sevenDaysAgo = DateTime.Today.AddDays(-7);
            
            List<CameraInfo> cameraInfosList = (from camera in _context.Camera.ToList()
                let sightingCount = (
                    from sightings in _context.Sightings
                    where sightings.CameraId == camera.Id &&
                          sightings.CaptureMoment > sevenDaysAgo
                    select sightings).Count() 
                select new CameraInfo(camera.Name, camera.Latitude, camera.Longitude, camera.RestrictedZone,
                    Url.Action("Details", "Cameras", new {id = camera.Id}), sightingCount)).ToList();


            DashboardViewModel vm = new DashboardViewModel{
                Cameras = cameraInfosList,
                BingApiKey = _bingSettings.Value.ApiKey
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}