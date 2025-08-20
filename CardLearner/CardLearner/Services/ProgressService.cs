using CardLearner.Data;
using CardLearner.Models;
using Microsoft.EntityFrameworkCore;

namespace CardLearner.Services;

public class ProgressService : IProgressService
{
    private readonly ApplicationDbContext _context;

    public ProgressService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserProgress?> GetProgressAsync(int userId, string quizName)
    {
        return await _context.UserProgresses
            .FirstOrDefaultAsync(p => p.UserId == userId && p.QuizName == quizName);
    }

    public async Task SaveProgressAsync(UserProgress progress)
    {
        var existing = await _context.UserProgresses
            .FirstOrDefaultAsync(p => p.UserId == progress.UserId && p.QuizName == progress.QuizName);

        if (existing == null)
        {
            _context.UserProgresses.Add(progress);
        }
        else
        {
            existing.CurrentQuestionIndex = progress.CurrentQuestionIndex;
            existing.Score = progress.Score;
        }

        await _context.SaveChangesAsync();
    }
}

