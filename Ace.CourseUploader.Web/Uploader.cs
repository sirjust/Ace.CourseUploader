using Ace.CourseUploader.Data.Models;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

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

            _driver.FindElement(By.Id("title")).SendKeys(lesson.CourseName);
            _driver.FindElement(By.Id("tab-sfwd-lessons-settings")).Click();
            _driver.FindElement(By.CssSelector("#select2-learndash-lesson-access-settings_course-container")).Click();
            _driver.FindElement(By.XPath($"//li[text() = '{lesson.CourseName}']")).Click();

            _driver.FindElement(By.Id("publish")).Click();
        }

        public void CreateQuiz(Quiz quiz)
        {
            Console.WriteLine($"Creating quiz with name {quiz.Title}");
            _driver.Url = _configuration["QuizPageUrl"];

            _driver.FindElement(By.Id("title")).SendKeys(quiz.Title);

            // Associate quiz to both lesson and course
            _driver.FindElement(By.Id("tab-sfwd-quiz-settings")).Click();
            _driver.FindElement(By.Id("select2-learndash-quiz-access-settings_course-container")).Click();
            _driver.FindElement(By.XPath($"//li[text() = '{quiz.Questions.FirstOrDefault().CourseName}']")).Click();

            _driver.FindElement(By.Id("tab-learndash_quiz_builder")).Click();

        }
    }
}
