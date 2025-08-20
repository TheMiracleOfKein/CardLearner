using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CardLearner.Controllers
{
    [Route("api/quiz")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        [HttpPost("submit")]
        public IActionResult Submit([FromBody] List<QuizResult> results)
        {
            if (results == null || results.Count == 0)
            {
                return BadRequest("No results provided.");
            }

            var summary = new
            {
                total = results.Count,
                correct = results.Count(r => r.IsCorrect)
            };

            return Ok(summary);
        }
    }

    public class QuizResult
    {
        public int QuestionId { get; set; }
        public string SelectedAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
