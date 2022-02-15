using Ace.CourseUploader.Data.Models;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;

namespace Ace.CourseUploader.Web
{
    public class Uploader : IUploader
    {
        IWebDriver _driver;
        WebDriverWait _wait;
        IConfiguration _configuration;

        public Uploader(IWebDriver driver, WebDriverWait wait)
        {
            _driver = driver;
            _wait = wait;

            _configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets<Uploader>()
            .Build();
        }

        public void Login()
        {
            _driver.Manage().Window.Maximize();
            _driver.Url = _configuration["LoginUrl"];

            _driver.FindElement(By.Id("user_login")).SendKeys(_configuration["Id"]);
            _driver.FindElement(By.Id("user_pass")).SendKeys(_configuration["Password"]);

            _driver.FindElement(By.Id("wp-submit")).Click();
        }

        public void CreateCourse(Course course)
        {
            Console.WriteLine($"Creating course with name {course.CourseName}");
            _driver.Url = _configuration["CoursePageUrl"];

            _driver.FindElement(By.Id("title")).SendKeys(course.CourseName);
            _driver.FindElement(By.Id("publish")).Click();
        }

        public void CreateLesson(Lesson lesson)
        {
            Console.WriteLine($"Creating lesson with name {lesson.CourseName}");
            _driver.Url = _configuration["LessonPageUrl"];

            _driver.FindElement(By.Id("title")).SendKeys(lesson.Name);
            _driver.FindElement(By.Id("tab-sfwd-lessons-settings")).Click();
            _driver.FindElement(By.CssSelector("#select2-learndash-lesson-access-settings_course-container")).Click();
            _wait.Until(d => d.FindElement(By.XPath($"//li[text() = '{lesson.CourseName}']"))).Click();

            _driver.FindElement(By.Id("publish")).Click();
        }

        public void CreateQuiz(Quiz quiz)
        {
            Console.WriteLine($"Creating quiz with name {quiz.Name}");
            _driver.Url = _configuration["QuizPageUrl"];

            _driver.FindElement(By.Id("title")).SendKeys(quiz.Name);

            #region quiz association
            // Associate quiz to both lesson and course
            _driver.FindElement(By.Id("tab-sfwd-quiz-settings")).Click();
            _driver.FindElement(By.Id("select2-learndash-quiz-access-settings_course-container")).Click();
            _wait.Until(d => d.FindElement(By.XPath($"//li[text() = '{quiz.CourseName}']"))).Click();

            Thread.Sleep(1000); // Here the clicking may be too quick for it to populate
            _driver.FindElement(By.Id("select2-learndash-quiz-access-settings_lesson-container")).Click();

            _wait.Until(d => d.FindElement(By.XPath($"//li[text() = '{quiz.LessonName}']"))).Click();
            #endregion

            var passingPercentageElement = _driver.FindElement(By.Id("learndash-quiz-progress-settings_passingpercentage"));
            passingPercentageElement.Clear();
            passingPercentageElement.SendKeys(quiz.PassPercentage.ToString());

            //_driver.FindElement(By.Id("tab-learndash_quiz_builder")).Click();

            //var count = 1;
            //foreach(var question in quiz.Questions)
            //{
            //    CreateQuestion(question, count);
            //    count++;
            //}

            _driver.FindElement(By.Id("publish")).Click();
        }

        public void CreateQuestion(Question question)
        {
            Console.WriteLine($"Creating question with text {question.QuestionText}");

            _driver.Url = _configuration["QuestionPageUrl"];

            _driver.FindElement(By.Id("title")).SendKeys($"{question.QuizNames.FirstOrDefault()} Question {question.QuestionNumber}");
            _driver.FindElement(By.ClassName("wp-editor-area")).Click();
            _driver.FindElement(By.ClassName("wp-editor-area")).SendKeys(question.QuestionText);

            // We need to click the `Add new answer` button so all the text fields are revealed. There is already one field visible, hence the subtraction
            var answerCount = question.Answers.Count;
            while (answerCount > 1)
            {
                Thread.Sleep(1000);
                var button = _wait.Until(d => d.FindElement(By.CssSelector(".classic_answer > .button-primary.addAnswer")));
                button.Click();
                answerCount--;
            }

            // _driver.FindElement(By.CssSelector(".answerList.ui-sortable"))

            var textareas = _driver.FindElements(By.CssSelector(".large-text.wpProQuiz_text"));

            // It appears the first textarea is inactive, so we start with element 1, but the answers are still 0-based
            for (int i = 1; i < textareas.Count; i++)
            {
                try
                {
                    textareas[i].Click();
                    textareas[i].SendKeys(question.Answers[i-1].Text);
                }

                catch
                {
                    continue;
                }
            }

            var correctButtons = _driver.FindElements(By.CssSelector(".wpProQuiz_classCorrect.wpProQuiz_checkbox"));
            correctButtons[Convert.ToInt32(question.CorrectAnswer) - 1].Click();

            // This matches a question to all its quizzes
            var selectElement = new SelectElement(_driver.FindElement(By.CssSelector("select[name='clms_questions_to_quiz[]']")));
            foreach (var quiz in question.QuizNames)
            {
                selectElement.SelectByText(quiz);
            }

            var element = _driver.FindElement(By.Id("publish"));
            Actions actions = new Actions(_driver);
            actions.MoveToElement(element);
            actions.Perform();
            element.Click();
        }
    }
}
