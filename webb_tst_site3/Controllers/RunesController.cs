using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site.Controllers
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
        [HttpGet("runes/random")]
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

            var random = new Random();
            var rune = runes[random.Next(runes.Count)];

            return Ok(new
            {
                rune.Id,
                rune.Name,
                rune.ImageUrl,
                rune.BaseDescription,
                SphereDescriptions = rune.SphereDescriptions
                    .Select(sd => new { sd.SphereId, sd.Description })
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
                    rune.Order = i; // Добавьте поле Order в модель Rune
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