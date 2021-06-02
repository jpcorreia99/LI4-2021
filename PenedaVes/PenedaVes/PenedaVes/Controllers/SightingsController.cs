using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PenedaVes.Data;
using PenedaVes.Models;

namespace PenedaVes.Controllers
{
    [Route("api/[action]")]
    [ApiController] //This attribute indicates that the controller responds to web API request

    // When the [action] token isn't in the route template, the action name is excluded from the route. 
    // That is, the action's associated method name isn't used in the matching route.
    public class SightingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SightingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/GetSpecies
        [HttpGet]
        public async Task<IActionResult> GetSpecies()
        {
            var query = from species in _context.Species
                select new {species.Id, species.CommonName};

            return Ok(await query.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetCameras()
        {
            var query = from camera in _context.Camera
                select new {camera.Id, camera.Name};

            return Ok(await query.ToListAsync());
        }
        
        // POST: api/ApiMovies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostSighting(Sighting sighting)
        {   
            var camera = await _context.Camera.FindAsync(sighting.CameraId);
            if (camera == null)
            {
                return NotFound("Camera does not exist");
            }
            
            var species = await _context.Species.FindAsync(sighting.SpeciesId);
            if (species == null)
            {
                return NotFound("Species does not exist");
            }
            
            _context.Sightings.Add(sighting);
            await _context.SaveChangesAsync();
            
            Console.WriteLine(sighting.ToString());
            return Ok(sighting);
        }


        //     // POST: api/TodoItems
        //     // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //     [HttpPost]
        //     public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        //     {
        //         _context.TodoItems.Add(todoItem);
        //         await _context.SaveChangesAsync();
        //
        //         return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        //     }
    }
}
