using Ace.CourseUploader.Data.Models;
using Ace.CourseUploader.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;

namespace Ace.CourseUploader.Web
{
    public class Uploader : IUploader
    {
        IWebDriver _driver;
        WebDriverWait _wait;

        public Uploader(IWebDriver driver, WebDriverWait wait)
        {
            _driver = driver;
            _wait = wait;
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

            //CreateCourse(package.Courses[0]);
            //CreateCourse(package.Courses[1]);
            //CreateLesson(package.Lessons[0]);
            //CreateLesson(package.Lessons[6]);
            //CreateQuiz(package.Quizzes[0]);
            //CreateQuiz(package.Quizzes[6]);
            //CreateQuestion(package.Questions[0]);

            //for (int i = 0; i < 10; i++)
            //{
            //    CreateQuestion(package.Questions[i]);
            //}
        }

        public void Login(string user, SecureString pw)
        {
            _driver.Manage().Window.Maximize();
            OpenUrl(Urls.LoginUrl);

            _driver.FindElement(By.Id("user_login")).SendKeys(user);
            _driver.FindElement(By.Id("user_pass")).SendKeys(Utilities.Security.SecureStringToString(pw));
            _driver.FindElement(By.Id("wp-submit")).Click();

            if(_driver.Url == Urls.LoginUrl) throw new Exception("Could not log in. Check credentials");
        }

        public void CreateCourse(Course course)
        {

            Console.WriteLine($"Creating course with name {course.CourseName}");
            OpenUrl(Urls.ListCoursesUrl);

            if (CourseAlreadyExists(course.CourseName)) 
            {
                Console.WriteLine($"Course {course.CourseName} already exists. Skipping creation...");
                return;
            }

            OpenUrl(Urls.NewCourseUrl);
            try
            {
                _driver.FindElement(By.Id("title")).SendKeys(course.CourseName);
                _driver.FindElement(By.Id("publish")).Click();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Something went wrong uploading {course.CourseName}. See message below. Attempting to continue...");
                Console.WriteLine(e.Message);
                // return;
            }

            // Check that the course is no longer a draft
            try
            {
                OpenUrl(Urls.ListCoursesUrl);
                var courseElement = _driver.FindElement(By.LinkText(course.CourseName));
                var parent = courseElement.FindElement(By.XPath("./.."));
                var draftElement = parent.FindElement(By.ClassName("post-state"));
                if (draftElement.Text.Contains("Draft"))
                {
                    courseElement.Click();
                    _driver.FindElement(By.Id("publish")).Click();
                }
            }
            catch
            {
                // Didn't find "Draft" text, so it was successfully published
            }
        }

        private bool CourseAlreadyExists(string courseName)
        {
            var exists = false;
            try
            {
                _driver.FindElement(By.Id("post-search-input")).SendKeys(courseName);
                _driver.FindElement(By.Id("search-submit")).Click();

                var element = _driver.FindElement(By.CssSelector("#the-list > tr > td"));
                if (!(element.Text == "No Courses found")) exists = true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return exists;
        }

        public void CreateLesson(Lesson lesson)
        {
            Console.WriteLine($"Creating lesson with name {lesson.FullLessonName}");
            OpenUrl(Urls.NewLessonUrl);

            try
            {
                _driver.FindElement(By.Id("title")).SendKeys(lesson.Name);
                _driver.FindElement(By.Id("tab-sfwd-lessons-settings")).Click();

                _driver.FindElement(By.CssSelector("#select2-learndash-lesson-access-settings_course-container")).Click(); 
                var searchField = _wait.Until(d => d.FindElement(By.ClassName("select2-search__field")));
                searchField.SendKeys(TruncateAmpersandOrApostrophe(lesson.CourseName));
                _wait.Until(d => d.FindElement(By.XPath($"//li[text() = '{lesson.CourseName}']"))).Click();

                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                js.ExecuteScript("window.scrollTo(0, 0)");

                Actions actions = new Actions(_driver);
                var publishElement = _driver.FindElement(By.Id("publish"));
                actions.MoveToElement(publishElement);
                actions.Perform();
                publishElement.Click();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Something went wrong uploading {lesson.FullLessonName}. See message below. Attempting to continue...");
                Console.WriteLine(e.Message);
                return;
            }

            // Check that the lesson is no longer a draft
            //try
            //{
            //    OpenUrl(Urls.ListLessonsUrl);
            //    var lessonElement = _driver.FindElement(By.LinkText(lesson.FullLessonName));
            //    var parent = lessonElement.FindElement(By.XPath("./.."));
            //    var draftElement = parent.FindElement(By.ClassName("post-state"));
            //    if (draftElement.Text.Contains("Draft"))
            //    {
            //        lessonElement.Click();
            //        _driver.FindElement(By.Id("publish")).Click();
            //    }
            //}
            //catch
            //{
            //    // Didn't find "Draft" text, so it was successfully published
            //}
            //try
            //{
            //    _driver.Navigate().Refresh();
            //    var publishElement = _driver.FindElement(By.Id("publish"));
            //    publishElement.Click();
            //}
            //catch { }
        }

        public void CreateQuiz(Quiz quiz)
        {
            Console.WriteLine($"Creating quiz with name {quiz.Name}");
            OpenUrl(Urls.NewQuizUrl);

            try
            {
                _driver.FindElement(By.Id("title")).SendKeys(quiz.Name);

                #region quiz association
                // Associate quiz to both lesson and course
                _driver.FindElement(By.Id("tab-sfwd-quiz-settings")).Click();
                _driver.FindElement(By.Id("select2-learndash-quiz-access-settings_course-container")).Click();
                var courseSearchField = _wait.Until(d => d.FindElement(By.CssSelector(".select2-container.select2-container--learndash.select2-container--open .select2-search__field")));
                courseSearchField.SendKeys(TruncateAmpersandOrApostrophe(quiz.CourseName));
                _wait.Until(d => d.FindElement(By.XPath($"//li[text() = '{quiz.CourseName}']"))).Click();

                Thread.Sleep(500); // Here the clicking may be too quick for it to populate
                _driver.FindElement(By.Id("select2-learndash-quiz-access-settings_lesson-container")).Click();
                var lessonSearchField = _wait.Until(d => d.FindElement(By.CssSelector(".select2-container.select2-container--learndash.select2-container--open .select2-search__field")));
                lessonSearchField.SendKeys(TruncateAmpersandOrApostrophe(quiz.LessonName));
                _wait.Until(d => d.FindElement(By.XPath($"//li[text() = '{quiz.LessonName}']"))).Click();
                #endregion

                var passingPercentageElement = _driver.FindElement(By.Id("learndash-quiz-progress-settings_passingpercentage"));
                passingPercentageElement.Clear();
                passingPercentageElement.SendKeys(quiz.PassPercentage.ToString());

                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                js.ExecuteScript("window.scrollTo(0, 0)");

                Actions actions = new Actions(_driver);
                var publishElement = _driver.FindElement(By.Id("publish"));
                actions.MoveToElement(publishElement);
                actions.Perform();
                publishElement.Click();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Something went wrong uploading {quiz.Name}. See message below. Attempting to continue...");
                Console.WriteLine(e.Message);
                return;
            }
        }

        public void CreateQuestion(Question question)
        {
            Console.WriteLine($"Creating question with text {question.QuestionText}");
            Actions actions = new Actions(_driver);
            OpenUrl(Urls.NewQuestionUrl);

            var lengthOfTitle = question.QuestionText.Length > 40 ? 40 : question.QuestionText.Length;
            _driver.FindElement(By.Id("title")).SendKeys(question.QuestionText.Substring(0, lengthOfTitle));

            var htmlButton = _driver.FindElement(By.Id("content-html"));
            actions.MoveToElement(htmlButton);
            actions.Perform();
            htmlButton.Click();

            var editorElement = _driver.FindElement(By.CssSelector("#content.wp-editor-area"));
            actions.MoveToElement(editorElement);
            actions.Perform();
            editorElement.SendKeys(question.QuestionText);

            // We need to click the `Add new answer` button so all the text fields are revealed. There is already one field visible, hence the subtraction
            var answerCount = question.Answers.Count;
            // They have implemented a dropdown for answers; we need to open this up
            var textareas = _driver.FindElements(By.CssSelector(".large-text.wpProQuiz_text"));
            var maxAttempts = 3;

            while (textareas.Count != answerCount + 1 && maxAttempts > 0)
            {
                _driver.FindElement(By.CssSelector("#learndash_question_answers .handlediv")).Click();
                maxAttempts--;

                try
                {
                    while (answerCount > 1)
                    {
                        Thread.Sleep(200);
                        var addAnswerButton = _driver.FindElement(By.CssSelector(".classic_answer > .button-primary.addAnswer"));
                        actions.MoveToElement(addAnswerButton);
                        actions.Perform();
                        addAnswerButton.Click();
                        answerCount--;
                    }
                }
                catch
                {
                    continue;
                }

                Thread.Sleep(200);
            }

            textareas = _driver.FindElements(By.CssSelector(".large-text.wpProQuiz_text"));

            // It appears the first textarea is inactive, so we start with element 1, but the answers are still 0-based
            for (int i = 1; i < textareas.Count; i++)
            {
                try
                {
                    actions.MoveToElement(textareas[i]);
                    actions.Perform();
                    textareas[i].Click();
                    textareas[i].SendKeys(question.Answers[i-1].Text);
                }

                catch
                {
                    continue;
                }
            }

            var correctButtons = _driver.FindElements(By.CssSelector(".wpProQuiz_classCorrect.wpProQuiz_checkbox"));

            var correct = correctButtons[Convert.ToInt32(question.CorrectAnswer) - 1];

            IJavaScriptExecutor ex = (IJavaScriptExecutor)_driver;
            ex.ExecuteScript("arguments[0].click()", correct);

            // This matches a question to all its quizzes
            var quizSelectHtmlElement = _driver.FindElement(By.CssSelector("select[name='clms_questions_to_quiz[]']"));
            actions.MoveToElement(quizSelectHtmlElement);
            actions.Perform();

            var selectElement = new SelectElement(quizSelectHtmlElement);
            foreach (var quiz in question.QuizNames)
            {
                try
                {
                    selectElement.SelectByText(quiz);
                }
                catch
                {
                    Console.WriteLine($"Cannot insert question into quiz {quiz}. The quiz doesn't seem to exist. Attempting to continue...");
                    return;
                }
            }

            var element = _driver.FindElement(By.Id("publish"));
            actions.MoveToElement(element);
            actions.Perform();
            element.Click();
        }

        public bool AllCoursesUnique(List<Course> courses)
        {
            Console.WriteLine($"Checking all courses. Each new course should have a unique name");
            OpenUrl(Urls.ListCoursesUrl);

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
            OpenUrl(Urls.ListLessonsUrl);

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
            OpenUrl(Urls.ListQuizzesUrl);

            var quizNameElements = _driver.FindElements(By.CssSelector("#the-list > tr > .title > strong > a")).ToList();
            var lessonNameElements = _driver.FindElements(By.CssSelector("#the-list > tr > .lesson_topic > a")).ToList();
            var courseNameElements = _driver.FindElements(By.CssSelector("#the-list > tr > .course > a")).ToList();

            if (!(courseNameElements.Count == lessonNameElements.Count && lessonNameElements.Count == quizNameElements.Count))
            {
                var error = "It appears not every quiz is mapped to a course. In order to proceed, all current quizzes must be mapped to a course and lesson";
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

        public void MinimizeWindow()
        {
            _driver.Manage().Window.Minimize();
        }

        public void OpenUrl(string url)
        {
            _driver.Url = url;

            try
            {
                // Check the presence of alert
                var alert = _driver.SwitchTo().Alert();

                // if present consume the alert
                alert.Accept();
            }
            catch (NoAlertPresentException e) { }

            catch (Exception e) { Console.WriteLine(e.Message); }

            return;
        }

        public string TruncateAmpersandOrApostrophe(string name)
        {
            var firstAmpersand = name.IndexOf('&');
            var firstApostrophe = name.IndexOf('\'');
            if(firstAmpersand > -1 || firstApostrophe > -1)
            {
                if (firstAmpersand == -1) name = name.Substring(0, firstApostrophe);
                else if (firstApostrophe == -1) name = name.Substring(0, firstAmpersand);
                else
                {
                    name = name.Substring(0, firstAmpersand < firstApostrophe ? firstAmpersand : firstApostrophe);
                }
            }

            return name;
        }
    }
}
