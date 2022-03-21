using Ace.CourseUploader.Data.Models;
using System.Security;

namespace Ace.CourseUploader.Web
{
    public interface IUploader
    {
        void CreateCourse(Course course);
        void Login(string user, SecureString pw);
        void CreateLesson(Lesson lesson);
        void CreateQuiz(Quiz quiz);
        void CreateQuestion(Question question);
        void UploadAllMaterials(UploadPackage package);
        void ValidateUniqueNames(UploadPackage uploadPackage);
        void MinimizeWindow();
    }
}