using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using webb_tst_site3.Services;

namespace webb_tst_site3.Pages
{
    // Pages/Contact.cshtml.cs
    public class ContactModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public Feedback Feedback { get; set; }

        public ContactModel(AppDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                //_context.Feedbacks.Add(Feedback);
                await _context.SaveChangesAsync();

                // Отправка email
                await _emailSender.SendEmailAsync(
                    "ваш-email@example.com",
                    $"Новое сообщение от {Feedback.Name}",
                    Feedback.Message);

                return RedirectToPage("ContactConfirmation");
            }
            return Page();
        }
    }
}
