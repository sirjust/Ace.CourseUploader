namespace Ace.CourseUploader.Utilities
{
    public static class Urls
    {
        public static string LoginUrl { get => "https://new.anytimece.com/wp-login.php"; }
        public static string NewCourseUrl { get => "https://new.anytimece.com/wp-admin/post-new.php?post_type=sfwd-courses"; }
        public static string NewLessonUrl { get => "https://new.anytimece.com/wp-admin/post-new.php?post_type=sfwd-lessons"; }
        public static string NewQuizUrl { get => "https://new.anytimece.com/wp-admin/post-new.php?post_type=sfwd-quiz"; }
        public static string NewQuestionUrl { get => "https://new.anytimece.com/wp-admin/post-new.php?post_type=sfwd-question"; }
        public static string ListCoursesUrl { get => "https://new.anytimece.com/wp-admin/edit.php?post_type=sfwd-courses"; }
        public static string ListLessonsUrl { get => "https://new.anytimece.com/wp-admin/edit.php?post_type=sfwd-lessons"; }
        public static string ListQuizzesUrl { get => "https://new.anytimece.com/wp-admin/edit.php?post_type=sfwd-quiz"; }
    }
}
