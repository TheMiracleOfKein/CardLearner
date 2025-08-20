using CardLearner.Models;

namespace CardLearner.Services;

public interface IProgressService
{
    Task<UserProgress?> GetProgressAsync(int userId, string quizName);

    Task SaveProgressAsync(UserProgress progress);
}

