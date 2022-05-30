using CommandLine;

namespace Ace.CourseUploader
{
    public class Options
    {
        [Option('o', "only", HelpText = "Whether to run only one portion; options are courses, lessons, quizzes, and questions", Required = false)]
        public string Only { get; set; }

        [Option('l', "link", HelpText = "Setting to false will attempt to publish all questions without linking them to the respective quiz", Required = false)]
        public bool? LinkQuestion { get; set; }
    }
}
