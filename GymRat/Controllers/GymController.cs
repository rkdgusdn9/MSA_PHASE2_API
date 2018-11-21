using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymRat.Models;

namespace GymRat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymController : ControllerBase
    {
        private readonly GymRatContext _context;

        public GymController(GymRatContext context)
        {
            _context = context;
        }

        // GET: api/Gym
        [HttpGet]
        public IEnumerable<GymItem> GetGymItem()
        {
            return _context.GymItem;
        }

        // GET: api/Gym/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGymItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var gymItem = await _context.GymItem.FindAsync(id);

            if (gymItem == null)
            {
                return NotFound();
            }

            return Ok(gymItem);
        }

        // PUT: api/Gym/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGymItem([FromRoute] int id, [FromBody] GymItem gymItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != gymItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(gymItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GymItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Gym
        [HttpPost]
        public async Task<IActionResult> PostGymItem([FromBody] GymItem gymItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.GymItem.Add(gymItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGymItem", new { id = gymItem.Id }, gymItem);
        }

        // DELETE: api/Gym/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGymItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var gymItem = await _context.GymItem.FindAsync(id);
            if (gymItem == null)
            {
                return NotFound();
            }

            _context.GymItem.Remove(gymItem);
            await _context.SaveChangesAsync();

            return Ok(gymItem);
        }

        // GET: api/Gym/BigPart
        [Route("BigPart")]
        [HttpGet]
        public async Task<List<string>> GetBigpart()
        {
            var gym = (from m in _context.GymItem
                         select m.BigPart).Distinct();

            var returned = await gym.ToListAsync();

            return returned;
        }

        private bool GymItemExists(int id)
        {
            return _context.GymItem.Any(e => e.Id == id);
        }
    }
}