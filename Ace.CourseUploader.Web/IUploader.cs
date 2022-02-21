using Ace.CourseUploader.Data.Models;

namespace Ace.CourseUploader.Web
{
    public interface IUploader
    {
        void CreateCourse(Course course);
        void Login();
        void CreateLesson(Lesson lesson);
        void CreateQuiz(Quiz quiz);
        void CreateQuestion(Question question);
        void UploadAllMaterials(UploadPackage package);
    }
}