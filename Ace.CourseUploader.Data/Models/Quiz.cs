using System.Collections.Generic;

namespace Ace.CourseUploader.Data.Models
{
    public class Quiz
    {
        public string CourseId { get; set; }
        public string LessonId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public List<Question> Questions { get; set; }
    }
}
