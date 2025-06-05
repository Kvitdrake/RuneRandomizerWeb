using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webb_tst_site3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RunesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Random _random = new();

        public RunesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomRune([FromQuery] int? sphereId = null)
        {
            var query = _context.Runes
                .Include(r => r.SphereDescriptions)
                .ThenInclude(sd => sd.Sphere)
                .AsQueryable();

            if (sphereId.HasValue)
            {
                query = query.Where(r => r.SphereDescriptions.Any(sd => sd.SphereId == sphereId.Value));
            }

            var runes = await query.ToListAsync();
            if (!runes.Any()) return NotFound();

            var rune = runes[_random.Next(runes.Count)];
            var description = rune.BaseDescription;
            var sphereName = "";

            if (sphereId.HasValue)
            {
                var sphereDesc = rune.SphereDescriptions.FirstOrDefault(sd => sd.SphereId == sphereId.Value);
                description = sphereDesc?.Description ?? description;
                sphereName = sphereDesc?.Sphere?.Name ?? "";
            }

            return Ok(new
            {
                rune.Id,
                rune.Name,
                rune.ImageUrl,
                description,
                sphereName,
                SphereDescriptions = rune.SphereDescriptions.Select(sd => new { sd.SphereId, sd.Description })
            });
        }

        [HttpPost("order")]
        public async Task<IActionResult> UpdateOrder([FromBody] List<int> runeIds)
        {
            try
            {
                var runes = await _context.Runes.ToListAsync();
                for (int i = 0; i < runeIds.Count; i++)
                {
                    var rune = runes.First(r => r.Id == runeIds[i]);
                    rune.Order = i;
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}