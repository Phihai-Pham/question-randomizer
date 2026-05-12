using DocumentFormat.OpenXml.Packaging;
using QuestionRandomizerApp.Models;
using System.Text.RegularExpressions;

namespace QuestionRandomizerApp.Services
{
    public class DocxParserService
    {
        public List<QAItem> Parse(string path)
        {
            List<QAItem> list = new();

            using (WordprocessingDocument doc =
                WordprocessingDocument.Open(path, false))
            {
                string text =
                    doc.MainDocumentPart.Document.Body.InnerText;

                list = ExtractQuestions(text);
            }

            return list;
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