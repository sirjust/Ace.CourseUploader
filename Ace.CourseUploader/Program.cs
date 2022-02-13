using Ace.CourseUploader.Data;
using Ace.CourseUploader.Data.Models;
using Ace.CourseUploader.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace Ace.CourseUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            
            var reader = (ISpreadsheetReader)host.Services.GetService(typeof(ISpreadsheetReader));
            reader.ReadSpreadsheet(".\\Course Upload MASTER 1.31.22.xlsx");

            IUploader uploader = (IUploader)host.Services.GetService(typeof(IUploader));
            uploader.Login();
            //foreach (var course in reader.UploadPackage.Courses)
            //{
            //    uploader.CreateCourse(course);
            //}
            //uploader.CreateCourse(reader.UploadPackage.Courses[0]);
            //uploader.CreateLesson(reader.UploadPackage.Lessons[0]);
            //uploader.CreateQuiz(reader.UploadPackage.Quizzes[0]);

            //uploader.CreateQuestion(reader.UploadPackage.Quizzes[0].Questions[0]);
            reader.UploadPackage.Quizzes[0].Questions.ForEach(uploader.CreateQuestion);
            //foreach(var question in reader.UploadPackage.Quizzes[0].Questions)
            //{
            //    uploader.CreateQuestion(question);
            //}

            //foreach (var lesson in reader.UploadPackage.Lessons)
            //{
            //    uploader.CreateLesson(lesson);
            //}
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices);

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            // This should suppress Chrome logs where it says a usb device isn't functioning
            var ls = new List<string> { "enable-logging" };
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddExcludedArguments(ls);
            var driver = new ChromeDriver(options: chromeOptions);

            services.AddSingleton<UploadPackage>();
            services.AddSingleton<ISpreadsheetReader, SpreadsheetReader>();
            services.AddSingleton<IWebDriver>(driver);
            services.AddSingleton<WebDriverWait>(new WebDriverWait(driver, TimeSpan.FromSeconds(10)));
            services.AddScoped<IUploader, Uploader>();
        }
    }
}
