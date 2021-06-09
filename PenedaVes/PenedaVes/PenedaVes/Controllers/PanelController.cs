using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PenedaVes.Data;
using PenedaVes.Models;
using PenedaVes.ViewModels;

namespace PenedaVes.Controllers
{
    [Authorize(Roles = "Root")]
    public class PanelController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PanelController(AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> users = await _userManager.Users.ToListAsync();

            List<UserBox> userBoxes = new List<UserBox>();
            
            foreach(ApplicationUser user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Root")) continue;
                UserBox ub = new UserBox
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    IsChecked = await _userManager.IsInRoleAsync(user, "Admin")
                };
                    
                userBoxes.Add(ub);
            }

            return View(new RootViewModel {UserBoxesList = userBoxes});
        }

        [HttpPost]
        public async Task<IActionResult> ChangePermissions(RootViewModel vm)
        {
            foreach (UserBox ub in vm.UserBoxesList)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(ub.UserId);

                // if the permission were removed
                if (await _userManager.IsInRoleAsync(user, "Admin") && !ub.IsChecked)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
                else if (!await _userManager.IsInRoleAsync(user, "Admin")
                         && ub.IsChecked)  // if the permission were added
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
            
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index", "Home"); 
        }
        
    }
}