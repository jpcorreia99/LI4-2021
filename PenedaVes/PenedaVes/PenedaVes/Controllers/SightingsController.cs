using System;
using System.Collections.Generic;
using System.Linq;
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

    //     // GET: api/TodoItems/5
    //     [HttpGet("{id:long}")]
    //     public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
    //     {
    //         var todoItem = await _context.TodoItems.FindAsync(id);
    //
    //         if (todoItem == null)
    //         {
    //             return NotFound();
    //         }
    //
    //         return todoItem;
    //     }
    //
    //     // PUT: api/TodoItems/5
    //     // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //     [HttpPut("{id:long}")]
    //     public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
    //     {
    //         if (id != todoItem.Id)
    //         {
    //             return BadRequest();
    //         }
    //
    //         _context.Entry(todoItem).State = EntityState.Modified;
    //
    //         try
    //         {
    //             await _context.SaveChangesAsync();
    //         }
    //         catch (DbUpdateConcurrencyException)
    //         {
    //             if (!TodoItemExists(id))
    //             {
    //                 return NotFound();
    //             }
    //             else
    //             {
    //                 throw;
    //             }
    //         }
    //
    //         return NoContent();
    //     }
    //
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
    //
    //     // DELETE: api/TodoItems/5
    //     [HttpDelete("{id:long}")]
    //     public async Task<IActionResult> DeleteTodoItem(long id)
    //     {
    //         var todoItem = await _context.TodoItems.FindAsync(id);
    //         if (todoItem == null)
    //         {
    //             return NotFound();
    //         }
    //
    //         _context.TodoItems.Remove(todoItem);
    //         await _context.SaveChangesAsync();
    //
    //         return NoContent();
    //     }
    //
    //     private bool TodoItemExists(long id)
    //     {
    //         return _context.TodoItems.Any(e => e.Id == id);
    //     }
    }
}
