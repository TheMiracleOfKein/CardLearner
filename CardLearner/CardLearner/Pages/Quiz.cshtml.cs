using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CardLearner.Models;
using CardLearner.Services;
using System.Security.Claims;

namespace CardLearner.Pages
{
    public class QuizModel : PageModel
    {
        private readonly IProgressService _progressService;

        public QuizModel(IProgressService progressService)
        {
            _progressService = progressService;
        }

        public List<Question>? Questions { get; set; }

        public UserProgress? Progress { get; set; }

        public async Task OnGet()
        {
            var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "questions", "az-204.json");
            using (var jsonFile = System.IO.File.OpenRead(jsonFilePath))
            {
                var res = await JsonSerializer.DeserializeAsync<List<Question>>(jsonFile);
                Questions = res ?? new List<Question>();
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                Progress = await _progressService.GetProgressAsync(userId, "az-204");
            }
        }

        public async Task<IActionResult> OnPostSaveAsync(int currentIndex, int score)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Unauthorized();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _progressService.SaveProgressAsync(new UserProgress
            {
                UserId = userId,
                QuizName = "az-204",
                CurrentQuestionIndex = currentIndex,
                Score = score
            });

            return new JsonResult(new { success = true });
        }
    }
}