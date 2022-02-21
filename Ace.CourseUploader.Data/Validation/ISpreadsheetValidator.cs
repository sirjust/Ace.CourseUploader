using Ace.CourseUploader.Data.Models;
using System;
using System.Collections.Generic;

namespace Ace.CourseUploader.Data.Validation
{
    public interface ISpreadsheetValidator
    {
        ValidationResponse<Course> AreCoursesValid(List<Course> courses);
        ValidationResponse<Lesson> AreLessonsValid(List<Lesson> lessons);
        ValidationResponse<Question> AreQuestionsValid(List<Question> questions);
        ValidationResponse<Quiz> AreQuizzesValid(List<Quiz> quizzes);
        bool QuestionHasAnswers(Question question);
        bool QuestionHasCorrectAnswer(Question question);
        Tuple<ValidationResponse<Course>, ValidationResponse<Lesson>, ValidationResponse<Quiz>, ValidationResponse<Question>> ValidateSpreadsheet(UploadPackage package);
    }
}