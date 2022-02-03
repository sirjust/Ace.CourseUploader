using Ace.CourseUploader.Data.Models;
using ExcelToEnumerable;
using System;

namespace Ace.CourseUploader.Data
{
    public class SpreadsheetReader : ISpreadsheetReader
    {
        public void ReadSpreadsheet(string filePath)
        {
            Console.WriteLine("Reading spreadsheet", filePath);
            Console.WriteLine();

            var result = filePath.ExcelToEnumerable<Course>(
            x => x
            .Property(y => y.CourseName).UsesColumnNamed("Course Name")
            .Property(y => y.ProductName).UsesColumnNamed("Product Name")
            .Property(y => y.StudentQuizFileName).UsesColumnNamed("Student Quiz File Name")
            .Property(y => y.Category).Ignore()
            .Property(y => y.Author).Ignore()
            );
        }
    }
}
