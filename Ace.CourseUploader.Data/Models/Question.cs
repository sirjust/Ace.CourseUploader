using System.Collections.Generic;

namespace Ace.CourseUploader.Data.Models
{
    public class Question
    {
        public string QuizId { get; set; }
        public string Text { get; set; }
        public List<Answer> Answers { get; set; }
    }
}