using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CardLearner.Models;

namespace CardLearner.QuestionUpdater
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\.."));
            string urlsFilePath = Path.Combine(projectDirectory, "urls.txt");
            string questionsDirectory = Path.Combine(projectDirectory, "questions");
            string jsonFilePath = Path.Combine(questionsDirectory, "az-204.json");

            await EnsureDirectoriesExist(questionsDirectory);
            await EnsureFileExists(jsonFilePath);

            var questions = await FetchQuestionsFromJson(jsonFilePath);
            var urls = await File.ReadAllLinesAsync(urlsFilePath);

            foreach (var url in urls)
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    continue;
                }

                var (exists, questionNumber, topicNumber) = QuestionExists(questions, url);

                if (exists)
                {
                    Console.WriteLine($"Question #{questionNumber} in Topic #{topicNumber} already exists. Skipping...");
                    continue;
                }


                try
                {
                    var newQuestions = await new WebPageParser().FetchQuestionsFromWeb(url, questions);
                    foreach (var newQuestion in newQuestions)
                    {
                        var existingQuestion = questions.FirstOrDefault(q => q.Number == newQuestion.Number && q.Topic == newQuestion.Topic);
                        if (existingQuestion != null)
                        {
                            questions.Remove(existingQuestion);
                        }
                        questions.Add(newQuestion);
                        Console.WriteLine($"Successfully added a question by {url}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process {url}: {ex.Message}");
                }
            }

            await SaveQuestionsToJson(jsonFilePath, questions);

            Console.WriteLine("Questions updated successfully.");
        }

        static async Task EnsureDirectoriesExist(string questionsDirectory)
        {
            if (!Directory.Exists(questionsDirectory))
            {
                Directory.CreateDirectory(questionsDirectory);
            }
        }

        static async Task EnsureFileExists(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                using (File.Create(jsonFilePath)) { }
            }
        }

        static async Task<List<Question>> FetchQuestionsFromJson(string filePath)
        {
            if (new FileInfo(filePath).Length == 0)
            {
                return new List<Question>();
            }

            try
            {
                using var jsonFile = File.OpenRead(filePath);
                return await JsonSerializer.DeserializeAsync<List<Question>>(jsonFile);
            }
            catch (JsonException)
            {
                return new List<Question>();
            }
        }

        static async Task SaveQuestionsToJson(string filePath, List<Question> questions)
        {
            using var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await JsonSerializer.SerializeAsync(outputStream, questions);
        }

        static (bool exists, int questionNumber, int topicNumber) QuestionExists(List<Question> questions, string url)
        {
            try
            {
                var regex = new Regex(@"topic-(\d+)-question-(\d+)", RegexOptions.IgnoreCase);
                var match = regex.Match(url);

                if (match.Success)
                {
                    int topicNumber = int.Parse(match.Groups[1].Value);
                    int questionNumber = int.Parse(match.Groups[2].Value);

                    bool exists = questions.Any(q => q.Number == questionNumber && q.Topic == topicNumber);
  
                    return (exists, questionNumber, topicNumber);
                }

                return (false, 0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing URL: {url}. Exception: {ex.Message}");
                return (false, 0, 0);
            }
        }

    }
}
