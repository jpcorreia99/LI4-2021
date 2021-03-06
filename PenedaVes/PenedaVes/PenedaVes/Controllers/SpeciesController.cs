using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PenedaVes.Data;
using PenedaVes.Data.FileManager;
using PenedaVes.Data.Repository;
using PenedaVes.Models;
using PenedaVes.ViewModels;

namespace PenedaVes.Controllers
{
    [Authorize]
    public class SpeciesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileManager _fileManager;
        private readonly IRepository _repository;

        public SpeciesController(AppDbContext context, 
            UserManager<ApplicationUser> userManager,
            IRepository repository,
            IFileManager fileManager)
        {
            _context = context;
            _userManager = userManager;
            _repository = repository;
            _fileManager = fileManager;
        }

        // GET: Species
        public async Task<IActionResult> Index()
        {
            return View(await _context.Species.ToListAsync());
        }

        // GET: Species/Details/5
        public async Task<IActionResult> Details(int? id, DateTime beginningDate, DateTime endingDate)
        {
            if (id == null)
            {
                return NotFound();
            }

            var species = await _context.Species
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (species == null)
            {
                return NotFound();
            }
            
            if (endingDate.Equals(DateTime.MinValue))
            {
                endingDate = DateTime.Now.Date;
            }
            
            if (beginningDate.Equals(DateTime.MinValue))
            {
                beginningDate = DateTime.Today.AddDays(-7);
            }

            endingDate = endingDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59); // to include the day

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            
            List<Sighting> sightings = await _repository.GetSpeciesSightings(species, user, 
                beginningDate, endingDate);

            SpeciesDetailsViewModel vm = new SpeciesDetailsViewModel
            {
                Species = species,
                CapturedSightings = sightings,
                BarChart = _repository.GetBarChart(sightings, beginningDate, endingDate, true)
            };
            
            return View(vm);
        }

        // GET: Species/Create
        public IActionResult Create()
        {
            return View(new SpeciesViewModel());
        }

        // POST: Species/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpeciesViewModel vm)
        {
            Species species = new Species
            {
                Id = vm.Id,
                CommonName = vm.CommonName,
                ScientificName = vm.ScientificName,
                Description = vm.Description,
                IsPredatory = vm.IsPredatory,
                Image = await _fileManager.SaveImage(vm.Image)
            };
            
            _context.Species.Add(species);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Species/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var species = await _context.Species.FindAsync(id);
            if (species == null)
            {
                return NotFound();
            }
            
            
            return View(new SpeciesViewModel
            {
                Id = species.Id,
                CommonName = species.CommonName,
                ScientificName = species.ScientificName,
                Description = species.Description,
                IsPredatory = species.IsPredatory,
                CurrentImage =  species.Image
            });
        }

        // POST: Species/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,SpeciesViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(vm);
            
            var species = new Species
            {
                Id = vm.Id,
                CommonName = vm.CommonName,
                ScientificName = vm.ScientificName,
                Description = vm.Description,
                IsPredatory = vm.IsPredatory
            };

            try
            {
                if (vm.Image == null)
                    species.Image = vm.CurrentImage;
                else
                {
                    if (!string.IsNullOrEmpty(vm.CurrentImage)) // Deletes the old image
                        _fileManager.RemoveImage(vm.CurrentImage);
                
                    species.Image = await _fileManager.SaveImage(vm.Image);
                }
                
                _context.Update(species);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpeciesExists(species.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Species/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var species = await _context.Species
                .FirstOrDefaultAsync(m => m.Id == id);
            if (species == null)
            {
                return NotFound();
            }

            return View(species);
        }

        // POST: Species/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var species = await _context.Species.FindAsync(id);
            
            if (!string.IsNullOrEmpty(species.Image))
                _fileManager.RemoveImage(species.Image);
            
            _context.Species.Remove(species);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpeciesExists(int id)
        {
            return _context.Species.Any(e => e.Id == id);
        }
    }
}
