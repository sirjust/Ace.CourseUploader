using Ace.CourseUploader.Data.Models;
using ExcelToEnumerable;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ace.CourseUploader.Data
{
    public class SpreadsheetReader : ISpreadsheetReader
    {
        UploadPackage _package;
        public SpreadsheetReader(UploadPackage package)
        {
            _package = package;
        }

        public UploadPackage UploadPackage { get { return _package; } }

        public void ReadSpreadsheet(string filePath)
        {
            Console.WriteLine("Reading spreadsheet", filePath);
            Console.WriteLine();
            var rawQuestionData = GetRawQuestionData(filePath);
            SetTrueFalse(rawQuestionData);

            _package.Courses = GetCourses(filePath);
            _package.Lessons = GetLessons(_package.Courses);
            _package.Quizzes = GetQuizzes(rawQuestionData);

            foreach(var quiz in _package.Quizzes)
            {
                // This creates a deep copy, otherwise when we scrub, the short version won't get any questions at all
                // This is inefficient, so if performance starts suffering we can revisit this approach
                List<Question> questions = JsonConvert.DeserializeObject<List<Question>>(JsonConvert.SerializeObject(rawQuestionData));
                quiz.Questions.AddRange(questions.Where(x => x.QuizName.Contains(quiz.Title)));
                quiz.CourseId = quiz.LessonId = ScrubVersions(quiz.Title, questions.FirstOrDefault().CourseName);

                foreach(var question in quiz.Questions)
                {
                    question.QuizName = ScrubVersions(quiz.Title, question.QuizName);
                    question.CourseName = ScrubVersions(quiz.Title, question.CourseName);
                }
            }
        }

        private static List<Course> GetCourses(string filePath)
        {
            return filePath.ExcelToEnumerable<Course>(
            x => x
            .Property(y => y.CourseName).UsesColumnNamed("Course Name")
            .Property(y => y.ProductName).UsesColumnNamed("Product Name")
            .Property(y => y.StudentQuizFileName).UsesColumnNamed("Student Quiz File Name")
            .Property(y => y.Category).Ignore()
            .Property(y => y.Author).Ignore()
            ).ToList();
        }

        private static List<Question> GetRawQuestionData(string filePath)
        {
            return filePath.ExcelToEnumerable<Question>(
            x => x.UsingSheet("Quiz_Questions FOR TESTING ONLY")
            
            .Property(y => y.CourseName).UsesColumnNamed("Course Name")
            .Property(y => y.QuizName).UsesColumnNamed("Quiz Name")
            .Property(y => y.QuestionNumber).UsesColumnNamed("Question Number")
            .Property(y => y.PassPercentage).UsesColumnNamed("Pass Percentage")
            .Property(y => y.QuestionText).UsesColumnNamed("Question")
            .Property(y => y.Answer1).UsesColumnNamed("Answer 1")
            .Property(y => y.Answer2).UsesColumnNamed("Answer 2")
            .Property(y => y.Answer3).UsesColumnNamed("Answer 3")
            .Property(y => y.Answer4).UsesColumnNamed("Answer 4")
            .Property(y => y.Answer5).UsesColumnNamed("Answer 5")
            .Property(y => y.CorrectAnswer).UsesColumnNamed("Correct Answer")
            .Property(y => y.QuizQuestionImage).UsesColumnNamed("Quiz Question Image")
            .Property(y => y.QuizId).Ignore()
            ).ToList();
        }

        private static void SetTrueFalse(List<Question> questions)
        {
            foreach(var question in questions)
            {
                if(question.Answer1 == "1" && question.Answer2 == "0" && question.Answer3 == null && question.Answer4 == null && question.Answer5 == null)
                {
                    question.Answer1 = "TRUE";
                    question.Answer2 = "FALSE";
                }
            }
        }

        private static List<Lesson> GetLessons(List<Course> courses)
        {
            var lessons = new List<Lesson>();
            foreach (var course in courses)
            {
                lessons.Add(new Lesson { CourseName = course.CourseName });
            }
            return lessons;
        }

        private static List<Quiz> GetQuizzes(List<Question> rawQuestions)
        {
            var quizzes = new List<Quiz>();
            foreach(var question in rawQuestions)
            {
                List<string> referencedQuizzes = question.QuizName.Split(',').ToList();
                foreach (var quizName in referencedQuizzes)
                {
                    if (!quizzes.Any(q => q.Title == quizName))
                    {
                        quizzes.Add(new Quiz { Title = quizName });
                    }
                }
            }
            return quizzes;
        }

        private static string ScrubVersions(string quizName, string name)
        {
            if (!name.Contains(","))
            {
                return name;
            }
            else
            {
                if (quizName.Contains("LV"))
                {
                    var names = name.Split(',');
                    name = names.Where(x => x.Contains("LV")).FirstOrDefault();
                }
                else if (quizName.Contains("SV"))
                {
                    var names = name.Split(',');
                    name = names.Where(x => x.Contains("SV")).FirstOrDefault();
                }
            }
            return name;
        }
    }
}
