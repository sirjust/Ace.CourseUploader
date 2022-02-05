using Ace.CourseUploader.Data.Models;

namespace Ace.CourseUploader.Web
{
    public interface IUploader
    {
        void CreateCourse(Course course);
        void Login();
    }
}