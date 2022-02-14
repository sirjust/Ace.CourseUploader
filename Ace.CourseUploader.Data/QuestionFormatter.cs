using Ace.CourseUploader.Data.Models;
using System.Collections.Generic;

namespace Ace.CourseUploader.Data
{
    public class QuestionFormatter : IQuestionFormatter
    {

        public void SetTrueFalse(List<Question> questions)
        {
            foreach (var question in questions)
            {
                if (question.Answer1 == "1" && question.Answer2 == "0" && question.Answer3 == null && question.Answer4 == null && question.Answer5 == null)
                {
                    question.Answer1 = "TRUE";
                    question.Answer2 = "FALSE";
                }
            }
        }

        public void SetQuizNames(List<Question> questions)
        {
            foreach(var question in questions)
            {
                var splitQuizNames = question.UnsplitQuizName.Split(',');
                question.QuizNames.AddRange(splitQuizNames);
            }
        }

        public void PopulateAnswers(Question question)
        {
            if (question.Answer1?.Length > 0)
            {
                question.Answers.Add(new Answer { Text = question.Answer1 });
            }

            if (question.Answer2?.Length > 0)
            {
                question.Answers.Add(new Answer { Text = question.Answer2 });
            }

            if (question.Answer3?.Length > 0)
            {
                question.Answers.Add(new Answer { Text = question.Answer3 });
            }

            if (question.Answer4?.Length > 0)
            {
                question.Answers.Add(new Answer { Text = question.Answer4 });
            }

            if (question.Answer5?.Length > 0)
            {
                question.Answers.Add(new Answer { Text = question.Answer5 });
            }
        }
    }
}
