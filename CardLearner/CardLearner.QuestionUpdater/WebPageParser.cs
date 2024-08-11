using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using CardLearner.Models;

namespace CardLearner.QuestionUpdater
{
    public class WebPageParser
    {
        private readonly WebClient _webClient;
        private string _url;

        public WebPageParser()
        {
            _webClient = new WebClient();
        }

        public async Task<List<Question>> FetchQuestionsFromWeb(string url, List<Question> existingQuestions)
        {
            try
            {
                string html = await _webClient.GetHtmlWithRetry(url);
                _url = url;
                if (string.IsNullOrEmpty(html))
                {
                    return new List<Question>();
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var questions = new List<Question>();
                var maxId = existingQuestions.Count != 0 ? existingQuestions.Max(q => q.Id) : 0;

                var questionNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'question-discussion-header')]");
                if (questionNodes != null)
                {
                    foreach (var node in questionNodes)
                    {
                        var question = ExtractQuestion(node, ref maxId);
                        questions.Add(question);
                    }
                }

                return questions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch questions from {url}: {ex.Message}");
                return new List<Question>();
            }
        }

        private Question ExtractQuestion(HtmlNode node, ref int maxId)
        {
            var questionNumberText = node.SelectSingleNode(".//div").InnerText.Trim();
            var questionNumber = ExtractQuestionNumber(questionNumberText);
            var topicNumber = ExtractTopicNumber(questionNumberText);

            var questionBody = node.SelectSingleNode("../div[contains(@class, 'question-body')]");
            var questionTextNode = questionBody.SelectSingleNode(".//p[@class='card-text']");
            var questionText = questionTextNode.InnerText.Trim();

            var imageUrlNode = questionTextNode.SelectSingleNode(".//img");
            var imageUrl = imageUrlNode != null ? GetAbsoluteUrl(imageUrlNode.GetAttributeValue("src", string.Empty)) : string.Empty;

            var options = ExtractOptions(questionBody);
            var correctAnswer = ExtractCorrectAnswer(questionBody, options);
            var explanation = ExtractExplanation(questionBody);

            return new Question
            {
                Id = ++maxId,
                Url = _url,
                Text = questionText,
                Options = options,
                CorrectAnswer = correctAnswer,
                Number = questionNumber,
                Topic = topicNumber,
                Explanation = explanation,
                ImageUrl = imageUrl,
                ExplanationImageUrl = GetExplanationImageUrl(questionBody)
            };
        }

        private string GetExplanationImageUrl(HtmlNode questionBody)
        {
            var explanationImageNode = questionBody.SelectSingleNode(".//span[@class='correct-answer']//img");
            var res = explanationImageNode != null ? GetAbsoluteUrl(explanationImageNode.GetAttributeValue("src", string.Empty)) : string.Empty;

            return res;
        }

        private string GetAbsoluteUrl(string relativeUrl)
        {

            var baseUrl = "https://www.examtopics.com";
            return new Uri(new Uri(baseUrl), relativeUrl).ToString();
        }

        private List<string> ExtractOptions(HtmlNode questionBody)
        {
            var options = new List<string>();
            var optionNodes = questionBody.SelectNodes(".//li[contains(@class, 'multi-choice-item')]");

            if (optionNodes != null)
            {
                foreach (var optionNode in optionNodes)
                {
                    var optionLetter = optionNode.SelectSingleNode(".//span[@class='multi-choice-letter']").InnerText.Trim();
                    var optionTextNode = optionNode.SelectSingleNode(".//span[@class='multi-choice-letter']").NextSibling;
                    var optionText = optionTextNode != null ? Regex.Replace(optionTextNode.InnerText.Trim(), @"\s+", " ") : string.Empty;
                    options.Add($"{optionLetter} {optionText}");
                }
            }

            return options;
        }

        private static string ExtractCorrectAnswer(HtmlNode questionBody, List<string> options)
        {
            var correctAnswerNode = questionBody.SelectSingleNode(".//li[contains(@class, 'correct-hidden')]");
            if (correctAnswerNode != null)
            {
                var correctLetterNode = correctAnswerNode.SelectSingleNode(".//span[@class='multi-choice-letter']");
                if (correctLetterNode != null)
                {
                    var correctLetter = correctLetterNode.InnerText.Trim();
                    foreach (var option in options)
                    {
                        if (option.StartsWith(correctLetter))
                        {
                            return option;
                        }
                    }
                }
            }
            return string.Empty;
        }

        private static string ExtractExplanation(HtmlNode questionBody)
        {
            var explanationNode = questionBody.SelectSingleNode(".//span[@class='answer-description']");
            if (explanationNode != null)
            {
                var result = HtmlEntity.DeEntitize(explanationNode.InnerText.Trim());
                return result; 
            }
            return string.Empty;
        }

        private static int ExtractQuestionNumber(string text)
        {
            var questionNumberPart = text.Split(new[] { "Question #:" }, StringSplitOptions.None)[1];
            var questionNumber = int.Parse(questionNumberPart.Split(new[] { "Topic #:" }, StringSplitOptions.None)[0].Trim());
            return questionNumber;
        }

        private static int ExtractTopicNumber(string text)
        {
            var topicNumberPart = text.Split(new[] { "Topic #:" }, StringSplitOptions.None)[1].Trim();
            var topicNumber = int.Parse(topicNumberPart);
            return topicNumber;
        }
    }
}
