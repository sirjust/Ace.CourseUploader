using Ace.CourseUploader.Data.Models;

namespace Ace.CourseUploader.Data
{
    public interface ISpreadsheetReader
    {
        void ReadSpreadsheet(string filePath);
        UploadPackage UploadPackage { get; }
    }
}