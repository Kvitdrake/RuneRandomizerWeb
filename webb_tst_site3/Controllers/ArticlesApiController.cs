// ArticlesApiController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ArticlesApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArticlesApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder([FromBody] ArticleOrderUpdateModel model)
        {
            var article = await _context.Articles.FindAsync(model.Id);
            if (article == null)
                return NotFound();

            // Обновляем родителя, если он изменился
            if (article.ParentId != model.ParentId)
            {
                article.ParentId = model.ParentId;
            }

            // Получаем всех соседей в новом родителе
            var siblings = await _context.Articles
                .Where(a => a.ParentId == model.ParentId && a.Id != model.Id)
                .OrderBy(a => a.DisplayOrder)
                .ToListAsync();

            // Определяем новую позицию
            int newOrder;
            if (!string.IsNullOrEmpty(model.PrevId))
            {
                var prevArticle = siblings.FirstOrDefault(a => a.Id.ToString() == model.PrevId);
                newOrder = prevArticle?.DisplayOrder + 1 ?? 0;
            }
            else if (!string.IsNullOrEmpty(model.NextId))
            {
                var nextArticle = siblings.FirstOrDefault(a => a.Id.ToString() == model.NextId);
                newOrder = nextArticle?.DisplayOrder ?? 0;
            }
            else
            {
                newOrder = (int)(siblings.Any() ? siblings.Max(a => a.DisplayOrder) + 1 : 0);
            }

            // Обновляем порядок
            article.DisplayOrder = newOrder;

            // Сдвигаем другие элементы при необходимости
            if (siblings.Any(a => a.DisplayOrder >= newOrder))
            {
                foreach (var sibling in siblings.Where(a => a.DisplayOrder >= newOrder))
                {
                    sibling.DisplayOrder++;
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    public class ArticleOrderUpdateModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string? PrevId { get; set; }
        public string? NextId { get; set; }
    }
}