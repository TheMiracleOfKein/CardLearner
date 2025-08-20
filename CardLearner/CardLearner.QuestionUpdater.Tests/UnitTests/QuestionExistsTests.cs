using System;
using System.Collections.Generic;
using System.Reflection;
using CardLearner.Models;
using CardLearner.QuestionUpdater;
using Xunit;

namespace CardLearner.QuestionUpdater.Tests.UnitTests
{
    public class QuestionExistsTests
    {
        private static (bool exists, int questionNumber, int topicNumber) InvokeQuestionExists(List<Question> questions, string url)
        {
            var assembly = typeof(WebPageParser).Assembly;
            var programType = assembly.GetType("CardLearner.QuestionUpdater.Program", true);
            var method = programType.GetMethod("QuestionExists", BindingFlags.NonPublic | BindingFlags.Static);
            return ((bool, int, int))method.Invoke(null, new object[] { questions, url });
        }

        [Fact]
        public void ReturnsTrueForExistingQuestion()
        {
            var questions = new List<Question>
            {
                new Question { Number = 1, Topic = 2 }
            };
            var url = "https://example.com/topic-2-question-1/";
            var (exists, questionNumber, topicNumber) = InvokeQuestionExists(questions, url);

            Assert.True(exists);
            Assert.Equal(1, questionNumber);
            Assert.Equal(2, topicNumber);
        }

        [Theory]
        [InlineData("https://example.com/topic-2-question-99/")]
        [InlineData("https://example.com/not-a-valid-url")]
        [InlineData("")]
        public void ReturnsFalseForNonExistingOrInvalidUrl(string url)
        {
            var questions = new List<Question>();
            var (exists, questionNumber, topicNumber) = InvokeQuestionExists(questions, url);

            Assert.False(exists);
            Assert.Equal(0, questionNumber);
            Assert.Equal(0, topicNumber);
        }

        [Fact]
        public void DoesNotThrowOnBadFormat()
        {
            var questions = new List<Question>();
            var url = "bad-format";

            var exception = Record.Exception(() => InvokeQuestionExists(questions, url));
            Assert.Null(exception);
        }
    }
}
