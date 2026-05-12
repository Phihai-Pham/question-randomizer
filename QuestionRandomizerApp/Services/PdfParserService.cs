using QuestionRandomizerApp.Models;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace QuestionRandomizerApp.Services
{
    public class PdfParserService
    {
        public List<QAItem> Parse(string path)
        {
            StringBuilder sb = new();

            using (PdfDocument document = PdfDocument.Open(path))
            {
                foreach (var page in document.GetPages())
                {
                    sb.Append(page.Text);
                }
            }

            return ExtractQuestions(sb.ToString());
        }

        private List<QAItem> ExtractQuestions(string text)
        {
            List<QAItem> items = new();

            string pattern =
                @"(\d+\..*?)(Answer:|ANSWER:)(.*?)(?=\d+\.|$)";

            MatchCollection matches =
                Regex.Matches(text, pattern,
                RegexOptions.Singleline);

            int id = 1;

            foreach (Match match in matches)
            {
                items.Add(new QAItem
                {
                    Id = id++,
                    Question = match.Groups[1].Value.Trim(),
                    Answer = match.Groups[3].Value.Trim()
                });
            }

            return items;
        }
    }
}