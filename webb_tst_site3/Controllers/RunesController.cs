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

        private string GetBaseRuneName(string fullName)
        {
            return fullName.Replace("Перевернутая ", "")
                          .Replace("Обратная ", "")
                          .Trim();
        }
        [HttpGet("random")]
        public async Task<IActionResult> GetRandomRune([FromQuery] int? sphereId = null, [FromQuery] int count = 1)
        {
            try
            {
                const int dailySphereId = 4; // ID сферы "Руна дня"
                count = Math.Clamp(count, 1, 3); // Ограничиваем count от 1 до 3

                // Обработка запроса руны дня (особый случай)
                if (sphereId.HasValue && sphereId.Value == dailySphereId)
                {
                    var dailyRunes = await _context.Runes
                        .Include(r => r.SphereDescriptions)
                        .ThenInclude(sd => sd.Sphere)
                        .Where(r => r.SphereDescriptions.Any(sd => sd.SphereId == dailySphereId))
                        .ToListAsync();

                    if (!dailyRunes.Any())
                        return NotFound("Не найдено рун для дня");

                    var dailyRune = dailyRunes[_random.Next(dailyRunes.Count)];
                    var dailyDesc = dailyRune.SphereDescriptions.FirstOrDefault(sd => sd.SphereId == dailySphereId);

                    return Ok(new
                    {
                        dailyRune.Id,
                        dailyRune.Name,
                        dailyRune.ImageUrl,
                        description = dailyDesc?.Description ?? dailyRune.BaseDescription,
                        sphereName = dailyDesc?.Sphere?.Name ?? "Руна дня",
                        isDaily = true
                    });
                }

                // Основной запрос для обычных рун
                var query = _context.Runes
                    .Include(r => r.SphereDescriptions)
                    .ThenInclude(sd => sd.Sphere)
                    .AsQueryable();

                // Применяем фильтр по сфере, если указан (кроме сферы дня)
                if (sphereId.HasValue && sphereId.Value != dailySphereId)
                {
                    query = query.Where(r => r.SphereDescriptions.Any(sd => sd.SphereId == sphereId.Value));
                }

                var allRunes = await query.ToListAsync();
                if (!allRunes.Any())
                    return NotFound("Руны не найдены");

                // Выбираем случайные руны, избегая дублирования базовых имен
                var selectedRunes = new List<Rune>();
                var usedBaseNames = new HashSet<string>();

                // Перемешиваем руны случайным образом
                var shuffledRunes = allRunes.OrderBy(x => _random.Next()).ToList();

                foreach (var rune in shuffledRunes)
                {
                    if (selectedRunes.Count >= count) break;

                    var baseName = GetBaseRuneName(rune.Name);
                    if (!usedBaseNames.Contains(baseName))
                    {
                        selectedRunes.Add(rune);
                        usedBaseNames.Add(baseName);
                    }
                }

                // Если не удалось набрать нужное количество уникальных рун, добавляем любые
                if (selectedRunes.Count < count)
                {
                    var additionalRunes = shuffledRunes
                        .Where(r => !selectedRunes.Contains(r))
                        .Take(count - selectedRunes.Count);
                    selectedRunes.AddRange(additionalRunes);
                }

                var result = selectedRunes.Select(rune =>
                {
                    var description = rune.BaseDescription;
                    var sphereName = sphereId.HasValue ? "Выбранная сфера" : "Общее";

                    if (sphereId.HasValue)
                    {
                        var sphereDesc = rune.SphereDescriptions
                            .FirstOrDefault(sd => sd.SphereId == sphereId.Value);
                        description = sphereDesc?.Description ?? description;
                        sphereName = sphereDesc?.Sphere?.Name ?? sphereName;
                    }

                    return new
                    {
                        rune.Id,
                        rune.Name,
                        rune.ImageUrl,
                        description,
                        sphereName,
                        SphereDescriptions = rune.SphereDescriptions?
                            .Select(sd => new { sd.SphereId, sd.Description }) ?? Enumerable.Empty<object>()
                    };
                }).ToList();

                return Ok(count == 1 ? result.FirstOrDefault() : result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }
        [HttpPost("order")]
        public async Task<IActionResult> UpdateOrder([FromBody] List<int> runeIds)
        {
            try
            {
                var runes = await _context.Runes.ToListAsync();
                foreach (var (runeId, index) in runeIds.Select((id, idx) => (id, idx)))
                {
                    var rune = runes.FirstOrDefault(r => r.Id == runeId);
                    if (rune != null)
                    {
                        rune.Order = index;
                    }
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при обновлении порядка: {ex.Message}");
            }
        }
    }
}