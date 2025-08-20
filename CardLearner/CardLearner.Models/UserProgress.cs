namespace CardLearner.Models;

public class UserProgress
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string QuizName { get; set; } = string.Empty;

    public int CurrentQuestionIndex { get; set; }

    public int Score { get; set; }

    public User? User { get; set; }
}

