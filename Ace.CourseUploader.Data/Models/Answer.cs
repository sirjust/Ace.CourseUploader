namespace Ace.CourseUploader.Data.Models
{
    public class Answer
    {
        public string QuestionId { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}