using System.Collections.Generic;

namespace Ace.CourseUploader.Data.Models
{
    public class UploadPackage
    {
        public List<Course> Courses { get; set; }
        public List<Lesson> Lessons { get; set; }
        public List<Quiz> Quizzes { get; set; }
    }
}
