using Ace.CourseUploader.Data.Models;
using ExcelToEnumerable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ace.CourseUploader.Data
{
    public class SpreadsheetReader : ISpreadsheetReader
    {
        UploadPackage _package;
        IQuestionFormatter _questionFormatter;
        public SpreadsheetReader(UploadPackage package, IQuestionFormatter questionFormatter)
        {
            _package = package;
            _questionFormatter = questionFormatter;
        }

        public UploadPackage UploadPackage { get { return _package; } }

        public void ReadSpreadsheet(string filePath)
        {
            Console.WriteLine($"Reading spreadsheet at {filePath}", filePath);
            Console.WriteLine();
            
            _package.Courses = GetCourses(filePath);
            _package.Quizzes = GetQuizzes(filePath);
            _package.Lessons = GetLessons(_package.Quizzes);

            var questions = GetRawQuestionData(filePath);
            _questionFormatter.SetTrueFalse(questions);
            _questionFormatter.SetQuizNames(questions);
            foreach (var question in questions) _questionFormatter.PopulateAnswers(question);

            _package.Questions = questions;
        }

        private static List<Course> GetCourses(string filePath)
        {
            try
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
            catch(Exception e)
            {
                FailedToReadSpreadsheet(e);
                throw;
            }

        }

        private static List<Question> GetRawQuestionData(string filePath)
        {
            try
            {
                return filePath.ExcelToEnumerable<Question>(
                    x => x.UsingSheet("Quiz_Questions FOR TESTING ONLY")

                    .Property(y => y.CourseName).UsesColumnNamed("Course Name")
                    .Property(y => y.UnsplitQuizName).UsesColumnNamed("Quiz Name")
                    .Property(y => y.QuestionNumber).UsesColumnNamed("Question Number")
                    .Property(y => y.QuestionText).UsesColumnNamed("Question")
                    .Property(y => y.Answer1).UsesColumnNamed("Answer 1")
                    .Property(y => y.Answer2).UsesColumnNamed("Answer 2")
                    .Property(y => y.Answer3).UsesColumnNamed("Answer 3")
                    .Property(y => y.Answer4).UsesColumnNamed("Answer 4")
                    .Property(y => y.Answer5).UsesColumnNamed("Answer 5")
                    .Property(y => y.CorrectAnswer).UsesColumnNamed("Correct Answer")
                    .Property(y => y.QuizQuestionImage).UsesColumnNamed("Quiz Question Image")
                    .Property(y => y.TruncatedQuizName).Ignore()
                    .Property(y => y.QuizNames).Ignore()
                    ).ToList();
            }
            catch (Exception e)
            {
                FailedToReadSpreadsheet(e);
                throw;
            }
        }

        private static List<Lesson> GetLessons(List<Quiz> quizzes)
        {
            try
            {
                var lessons = new List<Lesson>();
                foreach (var quiz in quizzes)
                {
                    lessons.Add(new Lesson { CourseName = quiz.CourseName, Name = quiz.LessonName });
                }
                return lessons;
            }
            catch (Exception e)
            {
                FailedToReadSpreadsheet(e);
                throw;
            }
        }

        private static List<Quiz> GetQuizzes(string filePath)
        {
            try
            {
                return filePath.ExcelToEnumerable<Quiz>(
                    x => x.UsingSheet("Course to Lesson to Quiz FTO")
                    .Property(y => y.Name).UsesColumnNamed("Quiz Name")
                    .Property(y => y.CourseName).UsesColumnNamed("Course Name")
                    .Property(y => y.LessonName).UsesColumnNamed("Lesson Name")
                    .Property(y => y.PassPercentage).UsesColumnNamed("Pass Percentage")
                    .Property(y => y.Author).Ignore()
                    ).ToList();
            }
            catch (Exception e)
            {
                FailedToReadSpreadsheet(e);
                throw;
            }
        }

        public static void FailedToReadSpreadsheet(Exception e)
        {
            Console.WriteLine($"There was an error reading the spreadsheet. This may be due to incorrect column names");
            Console.WriteLine($"See message: {e.Message}");
            Environment.Exit(-1);
        }
    }
}
