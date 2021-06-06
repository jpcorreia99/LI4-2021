using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PenedaVes.Data;
using PenedaVes.Data.Repository;
using PenedaVes.Models;
using PenedaVes.ViewModels;

namespace PenedaVes.Controllers
{
    public class SettingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository _repository;

        public SettingsController(AppDbContext context,
            UserManager<ApplicationUser> userManager,
            IRepository repository)
        {
            _context = context;
            _userManager = userManager;
            _repository = repository;
        }
        
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            
            SettingsViewModel vm = new SettingsViewModel
            {
                SpeciesBoxes = await GetSpeciesBoxList(user),
                CameraBoxes = await GetCameraBoxList(user),
                ReceiveSummary = user.ReceiveSummary,
                PhoneNumber = user.PhoneNumber,
                UseCellphone = user.UseCellphone,
                UseEmail = user.UseEmail
            };
            
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeSettings(SettingsViewModel vm)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            
            await HandleCameraSettings(vm, user);
            
            await HandleSpeciesSettings(vm, user);

            user.ReceiveSummary = vm.ReceiveSummary;
            user.PhoneNumber = vm.PhoneNumber;
            user.UseCellphone = vm.UseCellphone;
            user.UseEmail = vm.UseEmail;

            _context.Users.Update(user);
            
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index", "Home");
        }

        // Returns the list of Species box list, containing all species and highlighting the ones the user already follows
        private async Task<List<SpeciesBox>> GetSpeciesBoxList(ApplicationUser user)
        {
            List<Species> speciesList = await _context.Species.ToListAsync();
            List<int> currentlyFollowedSpeciesIds = _context.FollowedSpecies
                .Where(fs =>fs.UserId == user.Id)
                .Select(fs => fs.SpeciesId).Distinct().ToList();

            return  speciesList
                .Select(species => new SpeciesBox {SpeciesId = species.Id,
                    CommonName = species.CommonName, IsChecked = currentlyFollowedSpeciesIds.Contains(species.Id)}).ToList();

        }
        // Returns the list of Camera box list, containing all cameras and highlighting the ones the user already follows
        private async Task<List<CameraBox>> GetCameraBoxList(ApplicationUser user)
        {
            //Handle cameras
            List<Camera> cameraList;

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                cameraList = await _context.Camera.ToListAsync();
            }
            else
            {
                cameraList = await _context.Camera.Where(c => !c.RestrictedArea).ToListAsync();
            }
            
            List<int> currentlyFollowedCamerasIds = _context.FollowedCamera
                .Where(fc =>fc.UserId == user.Id)
                .Select(fc => fc.CameraId).Distinct().ToList();

            return cameraList
                .Select(camera => new CameraBox {CameraId = camera.Id,
                    Name = camera.Name, IsChecked = currentlyFollowedCamerasIds.Contains(camera.Id)}).ToList();
        }

        // Reads the Cameras the user has selected and updates the followed cameras
        private async Task HandleCameraSettings(SettingsViewModel vm, ApplicationUser user)
        {
            // handling species
            List<SpeciesBox> selectedSpecies = vm.SpeciesBoxes.Where(x => x.IsChecked).ToList();
            
            List<FollowedSpecies> followedSpeciesList = await _context.FollowedSpecies
                .Where(fs => fs.UserId.Equals(user.Id)).ToListAsync();
            
            
            _context.FollowedSpecies.RemoveRange(followedSpeciesList);
            
            foreach (var sb in selectedSpecies)
            {
                FollowedSpecies fs = new FollowedSpecies
                {
                    SpeciesId = sb.SpeciesId,
                    UserId = user.Id
                };
                    
                await _context.AddAsync(fs);
            }
        }

        private async Task HandleSpeciesSettings(SettingsViewModel vm, ApplicationUser user)
        {
            List<CameraBox> selectedCameras = vm.CameraBoxes.Where(x => x.IsChecked).ToList();
            
            List<FollowedCamera> followedCamerasList = await _context.FollowedCamera
                .Where(fc => fc.UserId.Equals(user.Id)).ToListAsync();
            
            
            _context.FollowedCamera.RemoveRange(followedCamerasList);
            
            foreach (var cb in selectedCameras)
            {
                FollowedCamera fc = new FollowedCamera
                {
                    CameraId = cb.CameraId,
                    UserId = user.Id
                };
                    
                await _context.AddAsync(fc);
            }
        }
    }
}