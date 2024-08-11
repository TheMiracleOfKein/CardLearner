using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CardLearner.Models;

namespace CardLearner.Pages
{
    public class QuizModel : PageModel
    {
        public List<Question>? Questions { get; set; }

        public async Task OnGet()
        {
            var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "questions", "az-204.json");
            using (var jsonFile = System.IO.File.OpenRead(jsonFilePath))
            {
                var res = await JsonSerializer.DeserializeAsync<List<Question>>(jsonFile);
                Questions = res ?? new List<Question>();
            }
        }
    }
}