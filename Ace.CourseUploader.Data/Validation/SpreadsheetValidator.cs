using Ace.CourseUploader.Data.Models;
using System;
using System.Collections.Generic;

namespace Ace.CourseUploader.Data.Validation
{
    public class SpreadsheetValidator : ISpreadsheetValidator
    {
        public Tuple<ValidationResponse<Course>, ValidationResponse<Lesson>, ValidationResponse<Quiz>, ValidationResponse<Question>> ValidateSpreadsheet(UploadPackage package)
        {
            Console.WriteLine("Validating spreadsheet");
            var courseValidation = AreCoursesValid(package.Courses);
            var lessonValidation = AreLessonsValid(package.Lessons);
            var quizValidation = AreQuizzesValid(package.Quizzes);
            var questionValidation = AreQuestionsValid(package.Questions);

            return new Tuple<ValidationResponse<Course>, ValidationResponse<Lesson>, ValidationResponse<Quiz>, ValidationResponse<Question>>(
                courseValidation,
                lessonValidation,
                quizValidation,
                questionValidation
                );
        }

        public ValidationResponse<Course> AreCoursesValid(List<Course> courses)
        {
            foreach (var course in courses)
            {
                if (!string.IsNullOrEmpty(course.CourseName) &&
                    !string.IsNullOrEmpty(course.Id) &&
                    course.Price > 0 &&
                    !string.IsNullOrEmpty(course.ProductName) &&
                    !string.IsNullOrEmpty(course.Sku) &&
                    !string.IsNullOrEmpty(course.State))
                {
                    continue;
                }
                else
                {
                    return new ValidationResponse<Course>
                    {
                        IsValid = false,
                        ValidationObject = course,
                        ValidationMessage = $"Course {course.CourseName} is invalid. Check its properties in the spreadsheet"
                    };
                }
            }
            return new ValidationResponse<Course> { IsValid = true };
        }

        public ValidationResponse<Lesson> AreLessonsValid(List<Lesson> lessons)
        {
            foreach(var lesson in lessons)
            {
                if (!string.IsNullOrEmpty(lesson.CourseName) &&
                    !string.IsNullOrEmpty(lesson.Name))
                {
                    continue;
                } 
                else
                {
                    return new ValidationResponse<Lesson>
                    {
                        IsValid = false,
                        ValidationObject = lesson,
                        ValidationMessage = $"Lesson {lesson.Name} is invalid. Check its properties in the spreadsheet"
                    };
                }
            }
            return new ValidationResponse<Lesson> { IsValid = true };
        }

        public ValidationResponse<Quiz> AreQuizzesValid(List<Quiz> quizzes)
        {
            foreach(var quiz in quizzes)
            {
                if(!string.IsNullOrEmpty(quiz.Name) &&
                    !string.IsNullOrEmpty(quiz.CourseName) &&
                    !string.IsNullOrEmpty(quiz.LessonName) &&
                    (quiz.PassPercentage > 0 && quiz.PassPercentage <= 100))
                {
                    continue;
                }
                else
                {
                    return new ValidationResponse<Quiz>
                    {
                        IsValid = false,
                        ValidationObject = quiz,
                        ValidationMessage = $"Quiz {quiz.Name} is invalid. Check its properties in the spreadsheet"
                    };
                }
            }
            return new ValidationResponse<Quiz> { IsValid = true };
        }

        public ValidationResponse<Question> AreQuestionsValid(List<Question> questions)
        {
            foreach(var question in questions)
            {
                if (!QuestionHasAnswers(question))
                {
                    return new ValidationResponse<Question>
                    {
                        IsValid = false,
                        ValidationObject = question,
                        ValidationMessage = $"Question with text '{question.QuestionText}' has no answers."
                    };
                }

                if (!QuestionHasCorrectAnswer(question))
                {
                    return new ValidationResponse<Question>
                    {
                        IsValid = false,
                        ValidationObject = question,
                        ValidationMessage = $"Question with text '{question.QuestionText}' has no correct answer."
                    };
                }

                if (!string.IsNullOrEmpty(question.QuestionText) &&
                    !string.IsNullOrEmpty(question.CourseName) &&
                    !string.IsNullOrEmpty(question.UnsplitQuizName) &&
                    question.QuestionNumber > 0)
                {
                    continue;
                }
                else
                {
                    return new ValidationResponse<Question>
                    {
                        IsValid = false,
                        ValidationObject = question,
                        ValidationMessage = $"Question with text {question.QuestionText} is invalid. Check its properties in the spreadsheet"
                    };
                }
            }
            return new ValidationResponse<Question> { IsValid = true };
        }

        public bool QuestionHasAnswers(Question question)
        {
            return question.Answers.Count > 0;
        }

        public bool QuestionHasCorrectAnswer(Question question)
        {
            return question?.CorrectAnswer <= question?.Answers?.Count && question?.CorrectAnswer != 0;
        }
    }
}
