using Microsoft.AspNetCore.Mvc;
using QuestionRandomizerApp.Models;
using QuestionRandomizerApp.Services;
using System.Text.Json;

namespace QuestionRandomizerApp.Controllers
{
    public class QuizController : Controller
    {
        private readonly DocxParserService _docx;
        private readonly PdfParserService _pdf;
        private readonly QuestionService _questionService;

        public QuizController(
            DocxParserService docx,
            PdfParserService pdf,
            QuestionService questionService)
        {
            _docx = docx;
            _pdf = pdf;
            _questionService = questionService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(
            IFormFile file,
            int questionCount)
        {

            //Clear old answer
            HttpContext.Session.Remove("Answers");
            HttpContext.Session.Remove("Questions");
            //


            if (file == null)
                return RedirectToAction("Index", "Home");

            string uploads =
                Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot/uploads");

            Directory.CreateDirectory(uploads);

            string filePath =
                Path.Combine(uploads, file.FileName);

            using (var stream = new FileStream(filePath,
                FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            List<QAItem> allQuestions = new();

            if (file.FileName.EndsWith(".docx"))
            {
                allQuestions = _docx.Parse(filePath);
            }
            else if (file.FileName.EndsWith(".pdf"))
            {
                allQuestions = _pdf.Parse(filePath);
            }

            if (questionCount == -1)
            {
                questionCount = allQuestions.Count;
            }

            if (questionCount <= 0)
            {
                questionCount = 1;
            }

            questionCount =
                Math.Min(questionCount,
                allQuestions.Count);

            var randomized =
                _questionService.Randomize(
                    allQuestions,
                    questionCount);

            HttpContext.Session.SetString(
                "Questions",
                JsonSerializer.Serialize(randomized));

            return RedirectToAction("Question", new { index = 0 });
        }

        public IActionResult Question(int index)
        {
            var json =
                HttpContext.Session.GetString("Questions");

            if (string.IsNullOrEmpty(json))
            {
                return RedirectToAction("Index", "Home");
            }

            var questions =
                JsonSerializer.Deserialize<List<QAItem>>(json);

            if (questions == null || questions.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            if (index >= questions.Count)
            {
                return RedirectToAction("Results");
            }

            ViewBag.Index = index;

            return View(questions[index]);
        }

        [HttpPost]
        public IActionResult SaveAnswer(
            int index,
            string userAnswer)
        {
            var questionJson =
    HttpContext.Session.GetString("Questions");

            if (string.IsNullOrEmpty(questionJson))
            {
                return RedirectToAction("Index", "Home");
            }

            var questions =
                JsonSerializer.Deserialize<List<QAItem>>(questionJson);

            if (questions == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var answersJson =
                HttpContext.Session.GetString("Answers");

            List<UserAnswer> answers;

            if (answersJson == null)
            {
                answers = new();
            }
            else
            {
                answers = JsonSerializer.Deserialize<List<UserAnswer>>(answersJson);
            }

            answers.Add(new UserAnswer
            {
                Question = questions[index].Question,
                CorrectAnswer = questions[index].Answer,
                UserResponse = userAnswer
            });

            HttpContext.Session.SetString(
                "Answers",
                JsonSerializer.Serialize(answers));

            return RedirectToAction("Question",
                new { index = index + 1 });
        }

        public IActionResult Results()
        {
            var json =
                HttpContext.Session.GetString("Answers");

            if (string.IsNullOrEmpty(json))
            {
                return RedirectToAction("Index", "Home");
            }

            var answers =
                JsonSerializer.Deserialize<List<UserAnswer>>(json);

            if (answers == null)
            {
                answers = new List<UserAnswer>();
            }

            return View(answers);
        }
    }
}