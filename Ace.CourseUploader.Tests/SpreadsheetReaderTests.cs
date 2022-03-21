using Ace.CourseUploader.Data;
using Ace.CourseUploader.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Ace.CourseUploader.Tests
{
    [TestClass]
    public class SpreadsheetReaderTests
    {
        [TestMethod]
        public void CourseNamesUnique_ShouldReturnTrue_WhenAllCourseNamesUnique()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course{CourseName = "test"},
                new Course{CourseName = "different"},
                new Course{CourseName = "alsoDifferent"}
            };

            // Act
            var actual = SpreadsheetReader.CourseNamesUnique(courses);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void CourseNamesUnique_ShouldReturnFalse_WhenNotUnique()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course{CourseName = "test"},
                new Course{CourseName = "different"},
                new Course{CourseName = "test"}
            };

            // Act
            var actual = SpreadsheetReader.CourseNamesUnique(courses);

            // Assert
            Assert.IsFalse(actual);
        }
    }
}
