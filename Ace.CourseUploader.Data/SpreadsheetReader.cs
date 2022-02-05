using Ace.CourseUploader.Data.Models;
using ExcelToEnumerable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ace.CourseUploader.Data
{
    public class SpreadsheetReader : ISpreadsheetReader
    {
        public List<Course> Courses { get; set; }
        public List<Question> Questions { get; set; }

        public void ReadSpreadsheet(string filePath)
        {
            Console.WriteLine("Reading spreadsheet", filePath);
            Console.WriteLine();

            Courses = GetCourses(filePath);
            Questions = GetRawQuestionData(filePath);
            SetTrueFalse(Questions);
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

        private void SetTrueFalse(List<Question> questions)
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
    }
}
