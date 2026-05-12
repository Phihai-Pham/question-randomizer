namespace QuestionRandomizerApp.Models
{
    public class QuizSession
    {
        public List<QAItem> Questions { get; set; } = new();

        public List<UserAnswer> Answers { get; set; } = new();
    }
}