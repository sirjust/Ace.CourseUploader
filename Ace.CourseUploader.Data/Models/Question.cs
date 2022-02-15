using System.Collections.Generic;
using System.Linq;

namespace Ace.CourseUploader.Data.Models
{
    public class Question
    {
        public string TruncatedQuizName { get => QuizNames.FirstOrDefault()[..^2]; }
        public List<Answer> Answers { get; set; } = new List<Answer>();
        public string CourseName { get; set; }
        public string UnsplitQuizName { get; set; }
        public List<string> QuizNames { get; set; } = new List<string>();
        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public string Answer5 { get; set; }
        public int CorrectAnswer { get; set; }
        public string QuizQuestionImage { get; set; }
    }
}