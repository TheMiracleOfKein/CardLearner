namespace CardLearner.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
        public int Number { get; set; }
        public int Topic { get; set; }
        public string Explanation { get; set; }
        public string? ImageUrl { get; set; } 
        public string? ExplanationImageUrl { get; set; }
        public string? SubTheme { get; set; } = "Unknown";
    }

    public class AnswerModel
    {
        public int QuestionId { get; set; }
        public string SelectedOption { get; set; }
    }
}
