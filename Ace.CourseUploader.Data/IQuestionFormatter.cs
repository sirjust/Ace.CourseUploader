using Ace.CourseUploader.Data.Models;
using System.Collections.Generic;

namespace Ace.CourseUploader.Data
{
    public interface IQuestionFormatter
    {
        void SetTrueFalse(List<Question> questions) { }
        void PopulateAnswers(Question question) { }
        void SetQuizNames(List<Question> questions) { }
    }
}