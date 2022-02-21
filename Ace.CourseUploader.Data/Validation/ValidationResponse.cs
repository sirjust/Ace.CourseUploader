namespace Ace.CourseUploader.Data.Validation
{
    public class ValidationResponse<T>
    {
        public T ValidationObject { get; set; }

        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }
    }
}
