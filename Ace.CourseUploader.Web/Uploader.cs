using Ace.CourseUploader.Data.Models;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
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
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        }

        public void UploadAllMaterials(UploadPackage package)
        {
            foreach (var course in package.Courses)
            {
                CreateCourse(course);
            }

            foreach (var lesson in package.Lessons)
            {
                CreateLesson(lesson);
            }

            foreach (var quiz in package.Quizzes)
            {
                CreateQuiz(quiz);
            }

            foreach (var question in package.Questions)
            {
                CreateQuestion(question);
            }

            //for (int i = 0; i < 10; i++)
            //{
            //    CreateQuestion(package.Questions[i]);
            //}

            //CreateCourse(package.Courses[0]);
            //CreateCourse(package.Courses[1]);
            //CreateLesson(package.Lessons[0]);
            //CreateLesson(package.Lessons[6]);
            //CreateQuiz(package.Quizzes[0]);
            //CreateQuiz(package.Quizzes[6]);
            //CreateQuestion(package.Questions[0]);
        }

        public void Login(string user, SecureString pw)
        {
            try
            {
                _driver.Manage().Window.Maximize();
                _driver.Url = _configuration["LoginUrl"];

                _driver.FindElement(By.Id("user_login")).SendKeys(user);
                var word = pw.ToString();
                _driver.FindElement(By.Id("user_pass")).SendKeys(Utilities.Security.SecureStringToString(pw));

                _driver.FindElement(By.Id("wp-submit")).Click();
            }

            catch
            {
                throw new Exception("Could not log in. Check credentials");
            }
        }

        public void CreateCourse(Course course)
        {

            Console.WriteLine($"Creating course with name {course.CourseName}");
            _driver.Url = _configuration["NewCourseUrl"];

            _driver.FindElement(By.Id("title")).SendKeys(course.CourseName);
            _driver.FindElement(By.Id("publish")).Click();
        }

        public void CreateLesson(Lesson lesson)
        {
            Console.WriteLine($"Creating lesson with name {lesson.CourseName}");
            _driver.Url = _configuration["NewLessonUrl"];

            _driver.FindElement(By.Id("title")).SendKeys(lesson.Name);
            _driver.FindElement(By.Id("tab-sfwd-lessons-settings")).Click();
            _driver.FindElement(By.CssSelector("#select2-learndash-lesson-access-settings_course-container")).Click();
            _wait.Until(d => d.FindElement(By.XPath($"//li[text() = '{lesson.CourseName}']"))).Click();

            _driver.FindElement(By.Id("publish")).Click();
        }

        public void CreateQuiz(Quiz quiz)
        {
            Console.WriteLine($"Creating quiz with name {quiz.Name}");
            _driver.Url = _configuration["NewQuizUrl"];

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

            _driver.Url = _configuration["NewQuestionUrl"];

            _driver.FindElement(By.Id("title")).SendKeys($"{question.TruncatedQuizName} Question {question.QuestionNumber}");
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

        public bool AllCoursesUnique(List<Course> courses)
        {
            Console.WriteLine($"\nChecking all courses. Each new course should have a unique name");
            _driver.Url = _configuration["ListCoursesUrl"];

            var list = _driver.FindElements(By.CssSelector("#the-list > tr > .title > strong > a")).ToList();

            foreach (var course in courses)
            {
                if (list.Any(x => x.GetAttribute("innerHTML") == course.CourseName))
                {
                    Console.WriteLine($"Course {course.CourseName} already exists.");
                    return false;
                }
            }
            return true;
        }

        public bool AllLessonsUnique(List<Lesson> lessons)
        {
            Console.WriteLine($"Checking all lessons. Each new lesson should have a unique name");
            _driver.Url = _configuration["ListLessonsUrl"];

            var lessonNameElements = _driver.FindElements(By.CssSelector("#the-list > tr > .title > strong > a")).ToList();
            var courseNameElements = _driver.FindElements(By.CssSelector("#the-list > tr > .course > a")).ToList();

            //foreach (var courseElement in courseNameElements) Console.WriteLine(courseElement.GetAttribute("innerHTML"));
            //foreach (var lessonElement in lessonNameElements) Console.WriteLine(lessonElement.GetAttribute("innerHTML"));

            if (courseNameElements.Count != lessonNameElements.Count)
            {
                var error = "It appears not every lesson is mapped to a course. In order to proceed, all current lessons must be mapped to a course";
                throw new Exception(error);
            }

            var names = new List<string>();
            for(int i = 0; i < courseNameElements.Count; i++)
            {
                names.Add($"{courseNameElements[i].GetAttribute("innerHTML")} {lessonNameElements[i].GetAttribute("innerHTML")}");
            }

            foreach (var lesson in lessons)
            {
                if (names.Any(x => x == lesson.FullLessonName))
                {
                    Console.WriteLine($"Lesson {lesson.FullLessonName} already exists.");
                    return false;
                }
            }
            return true;
        }

        public bool AllQuizzesUnique(List<Quiz> quizzes)
        {
            Console.WriteLine($"Checking all quizzes. Each new quiz should have a unique mapping to a lesson and course");
            _driver.Url = _configuration["ListQuizzesUrl"];

            var quizNameElements = _driver.FindElements(By.CssSelector("#the-list > tr > .title > strong > a")).ToList();
            var lessonNameElements = _driver.FindElements(By.CssSelector("#the-list > tr > .lesson_topic > a")).ToList();
            var courseNameElements = _driver.FindElements(By.CssSelector("#the-list > tr > .course > a")).ToList();

            if (!(courseNameElements.Count == lessonNameElements.Count && lessonNameElements.Count == quizNameElements.Count))
            {
                var error = "It appears not every quiz is mapped to a course. In order to proceed, all current lessons must be mapped to a course";
                throw new Exception(error);
            }

            var currentQuizzes = new List<Quiz>();
            for (int i = 0; i < courseNameElements.Count; i++)
            {
                currentQuizzes.Add(new Quiz
                {
                    Name = quizNameElements[i].GetAttribute("innerHTML"),
                    LessonName = lessonNameElements[i].GetAttribute("innerHTML"),
                    CourseName = courseNameElements[i].GetAttribute("innerHTML")
                });
            }

            foreach(var quiz in quizzes)
            {
                if (currentQuizzes.Any(x => x.Name == quiz.Name && x.LessonName == quiz.LessonName && x.CourseName == quiz.CourseName))
                {
                    Console.WriteLine($"Quiz {quiz.Name} already exists.");
                    return false;
                }
            }

            return true;
        }

        public void ValidateUniqueNames(UploadPackage uploadPackage)
        {
            if (AllCoursesUnique(uploadPackage.Courses) &&
            AllLessonsUnique(uploadPackage.Lessons) &&
            AllQuizzesUnique(uploadPackage.Quizzes)) return;
            else throw new Exception("All courses, lessons, and quizzes must be unique. See message above and fix spreadsheet");
        }
    }
}
