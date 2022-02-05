using Ace.CourseUploader.Data.Models;
using System.Collections.Generic;

namespace Ace.CourseUploader.Data
{
    public interface ISpreadsheetReader
    {
        List<Course> Courses { get; set; }
        List<Question> Questions { get; set; }
        void ReadSpreadsheet(string filePath);
    }
}