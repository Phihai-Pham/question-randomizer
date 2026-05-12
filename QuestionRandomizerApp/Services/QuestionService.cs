using QuestionRandomizerApp.Models;

namespace QuestionRandomizerApp.Services
{
    public class QuestionService
    {
        public List<QAItem> Randomize(
            List<QAItem> items,
            int count)
        {
            return items
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .ToList();
        }
    }
}