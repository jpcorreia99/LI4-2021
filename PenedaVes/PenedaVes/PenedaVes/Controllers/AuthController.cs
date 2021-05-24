using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using PenedaVes.Models;
using PenedaVes.ViewModels;

namespace PenedaVes.Controllers
{
    public class AuthController : Controller
    {
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;

        public AuthController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
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
            
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}