using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PenedaVes.Data;
using PenedaVes.Models;
using PenedaVes.ViewModels;

namespace PenedaVes.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(AppDbContext context,SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            var result = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, false, false);
            
            if (!result.Succeeded)
            {
                return View(vm);
            }
            
            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = new ApplicationUser
            {
                UserName = vm.Username,
                Email = vm.Email,
                ReceiveSummary = vm.ReceiveSummary,
                UseCellphone = false,
                UseEmail = true,
                PhoneNumber = vm.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded) return View(vm);

            List<Species> allSpecies = await _context.Species.ToListAsync();
            foreach (var fs in allSpecies.Select(species => new FollowedSpecies
            {
                SpeciesId = species.Id,
                UserId = user.Id
            }))
            {
                await _context.AddAsync(fs);
            }

            List<Camera> cameras = await _context.Camera.Where(c => !c.RestrictedArea).ToListAsync();
            foreach (var fc in cameras.Select(camera => new FollowedCamera
            {
                CameraId = camera.Id,
                UserId = user.Id
            }))
            {
                await _context.AddAsync(fc);
            }
            
            await _context.SaveChangesAsync();
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}