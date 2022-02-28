using CommandLine;

namespace Ace.CourseUploader
{
    public class Options
    {
        [Option('q', "questions", HelpText = "Whether to run only questions, options are Y and N", Required = false)]
        public string Questions { get; set; }
    }
}
