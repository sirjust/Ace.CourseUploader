using Ace.CourseUploader.Data;
using Ace.CourseUploader.Data.Models;
using Ace.CourseUploader.Data.Validation;
using Ace.CourseUploader.Web;
using CommandLine;
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
            var questionsOnly = false;
            CommandLine.Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o => {
                if (o.Questions == null) { }
                else if (o.Questions.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    questionsOnly = true;
                }
                else if (o.Questions.Equals("N", StringComparison.OrdinalIgnoreCase))
                {

                }
                else
                {
                    throw new Exception("Incorrect command line arguments, try running --help");
                }
            })
            .WithNotParsed(e => {
                throw new Exception("Incorrect command line arguments, try running --help");
            });

            var host = CreateHostBuilder(args).Build();
            
            var reader = (ISpreadsheetReader)host.Services.GetService(typeof(ISpreadsheetReader));
            var validator = (ISpreadsheetValidator)host.Services.GetService(typeof(ISpreadsheetValidator));

            reader.ReadSpreadsheet(".\\Course Uploads.xlsx");
            var validationResponse = validator.ValidateSpreadsheet(reader.UploadPackage);

            // Check whether the spreadsheet is valid
            if(!validationResponse.Item1.IsValid || !validationResponse.Item2.IsValid || !validationResponse.Item3.IsValid || !validationResponse.Item4.IsValid)
            {
                Console.WriteLine(validationResponse.Item1.ValidationMessage);
                Console.WriteLine(validationResponse.Item2.ValidationMessage);
                Console.WriteLine(validationResponse.Item3.ValidationMessage);
                Console.WriteLine(validationResponse.Item4.ValidationMessage);
                Environment.Exit(-1);
            }

            // Upload the data in the spreasheet
            IUploader uploader = (IUploader)host.Services.GetService(typeof(IUploader));

            Console.Write("Enter username: ");
            string user = Console.ReadLine();
            Console.Write("Enter password: ");
            var pw = Utilities.Security.GetPassword();

            try
            {
                uploader.Login(user, pw);
                if (questionsOnly)
                {
                    foreach(var question in reader.UploadPackage.Questions)
                    {
                        uploader.CreateQuestion(question);
                    }
                } else
                {
                    uploader.ValidateUniqueNames(reader.UploadPackage);
                    uploader.UploadAllMaterials(reader.UploadPackage);
                }
                Console.WriteLine("Upload successful");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(-1);
            }
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
            services.AddSingleton<IQuestionFormatter, QuestionFormatter>();
            services.AddSingleton<ISpreadsheetReader, SpreadsheetReader>();
            services.AddSingleton<ISpreadsheetValidator, SpreadsheetValidator>();
            services.AddSingleton<IWebDriver>(driver);
            services.AddSingleton<WebDriverWait>(new WebDriverWait(driver, TimeSpan.FromSeconds(10)));
            services.AddScoped<IUploader, Uploader>();
        }
    }
}
