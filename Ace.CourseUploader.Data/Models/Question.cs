using System.Collections.Generic;

namespace Ace.CourseUploader.Data.Models
{
    public class Question
    {
        public string QuizId { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();
        public string CourseName { get; set; }
        public string QuizName { get; set; }
        public int QuestionNumber { get; set; }
        public int PassPercentage { get; set; }
        public string QuestionText { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public string Answer5 { get; set; }
        public string CorrectAnswer { get; set; }
        public string QuizQuestionImage { get; set; }
    }
}