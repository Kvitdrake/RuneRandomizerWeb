// Pages/Admin/Quizzes/Answers/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace webb_tst_site3.Pages.Admin.Quizzes.Questions.Answers
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        // Параметры страницы
        [FromQuery]
        public int QuizId { get; set; }

        [FromQuery]
        public int? QuestionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchText { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortField { get; set; } = "QuestionOrder";

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; } = "asc";

        public PaginatedList<AnswerViewModel> Answers { get; set; }
        public Models.Quiz Quiz { get; set; }
        public Question Question { get; set; }

        public async Task OnGetAsync(int pageIndex = 1)
        {
            IQueryable<Answer> baseQuery = _context.Answers
                .Include(a => a.Question)
                .Include(a => a.Result)
                .Where(a => a.Question.QuizId == QuizId);

            // Фильтр по вопросу, если указан
            if (QuestionId.HasValue)
            {
                baseQuery = baseQuery.Where(a => a.QuestionId == QuestionId.Value);
                Question = await _context.Questions.FindAsync(QuestionId.Value);
            }

            // Поиск
            if (!string.IsNullOrEmpty(SearchText))
            {
                baseQuery = baseQuery.Where(a =>
                    a.Text.Contains(SearchText) ||
                    a.Result.Name.Contains(SearchText));
            }

            // Сортировка
            baseQuery = SortField switch
            {
                "Text" => SortDirection == "asc"
                    ? baseQuery.OrderBy(a => a.Text)
                    : baseQuery.OrderByDescending(a => a.Text),
                "Score" => SortDirection == "asc"
                    ? baseQuery.OrderBy(a => a.Score)
                    : baseQuery.OrderByDescending(a => a.Score),
                "Result" => SortDirection == "asc"
                    ? baseQuery.OrderBy(a => a.Result.Name)
                    : baseQuery.OrderByDescending(a => a.Result.Name),
                _ => SortDirection == "asc"
                    ? baseQuery.OrderBy(a => a.Question.Order).ThenBy(a => a.Id)
                    : baseQuery.OrderByDescending(a => a.Question.Order).ThenByDescending(a => a.Id)
            };

            // Получаем данные для отображения
            var answerViewModels = await baseQuery
                .Select(a => new AnswerViewModel
                {
                    Id = a.Id,
                    Text = a.Text,
                    Score = a.Score,
                    ResultName = a.Result.Name,
                    QuestionId = a.QuestionId,
                    QuestionText = a.Question.Text,
                    QuestionOrder = a.Question.Order
                })
                .ToListAsync();

            Answers = PaginatedList<AnswerViewModel>.Create(answerViewModels, pageIndex, 10);
            Quiz = await _context.Quizzes.FindAsync(QuizId);
        }

        public class AnswerViewModel
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public int Score { get; set; }
            public string ResultName { get; set; }
            public int QuestionId { get; set; }
            public string QuestionText { get; set; }
            public int QuestionOrder { get; set; }
        }
    }

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static PaginatedList<T> Create(List<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count;
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}