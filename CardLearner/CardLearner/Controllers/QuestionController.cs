using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CardLearner.Models;

namespace CardLearner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly string _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "questions", "az-204.json");

        [HttpPost("update-correct-answer")]
        public async Task<IActionResult> UpdateCorrectAnswer([FromBody] UpdateCorrectAnswerRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.NewCorrectAnswer))
            {
                return BadRequest("Invalid request data.");
            }

            var questions = await FetchQuestionsFromJson(_jsonFilePath);
            var question = questions.FirstOrDefault(q => q.Id == request.QuestionId);

            if (question == null)
            {
                return NotFound("Question not found.");
            }

            question.CorrectAnswer = request.NewCorrectAnswer;
            await SaveQuestionsToJson(_jsonFilePath, questions);

            return Ok(new { message = "Correct answer updated successfully." });
        }

        [HttpPost("update-explanation")]
        public async Task<IActionResult> UpdateExplanation([FromBody] UpdateExplanationModel model)
        {
            var questions = await FetchQuestionsFromJson(_jsonFilePath);
            var question = questions.FirstOrDefault(q => q.Id == model.QuestionId);
            if (question == null)
            {
                return NotFound();
            }

            question.Explanation = model.NewExplanation;

            await SaveQuestionsToJson(_jsonFilePath, questions);
            return Ok(new { success = true });
        }

        private async Task<List<Question>> FetchQuestionsFromJson(string filePath)
        {
            if (!System.IO.File.Exists(filePath) || new FileInfo(filePath).Length == 0)
            {
                return new List<Question>();
            }

            try
            {
                using var jsonFile = System.IO.File.OpenRead(filePath);
                return await JsonSerializer.DeserializeAsync<List<Question>>(jsonFile);
            }
            catch (JsonException)
            {
                return new List<Question>();
            }
        }

        private async Task SaveQuestionsToJson(string filePath, List<Question> questions)
        {
            using var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await JsonSerializer.SerializeAsync(outputStream, questions);
        }
    }

    public class UpdateCorrectAnswerRequest
    {
        public int QuestionId { get; set; }
        public string NewCorrectAnswer { get; set; }
    }

    public class UpdateExplanationModel
    {
        public int QuestionId { get; set; }
        public string NewExplanation { get; set; }
    }
}
