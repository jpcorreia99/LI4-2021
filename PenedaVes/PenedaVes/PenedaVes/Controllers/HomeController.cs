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
using PenedaVes.Data.Repository;
using PenedaVes.Models;
using PenedaVes.ViewModels;

namespace PenedaVes.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IOptions<BingSettings> _bingSettings;
        private readonly IFileManager _fileManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository _repository;
        public HomeController(
            IOptions<BingSettings> bingSettings,
            IFileManager fileManager,
            UserManager<ApplicationUser> userManager,
            IRepository repository)
        {
            _bingSettings = bingSettings;
            _fileManager = fileManager;
            _userManager = userManager;
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            List<Camera> followedCameras = await _repository.GetFollowedCameras(user);

            List<Species> followedSpecies = await _repository.GetFollowedSpecies(user);

            List<CameraInfo> camerasInfoList = _repository.GetCameraInfo(followedCameras, followedSpecies);

            List<Sighting> sightingList = await _repository.GetFollowedSightings(followedCameras, followedSpecies);
            
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