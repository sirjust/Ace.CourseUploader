namespace Ace.CourseUploader.Data.Models
{
    public class Lesson
    {
        public string Name { get; set; }
        public string CourseName { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string FullLessonName { get => $"{CourseName} {Name}"; }
    }
}
