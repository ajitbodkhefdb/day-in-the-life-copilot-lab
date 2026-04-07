using ContosoUniversity.Core.Models;
using Xunit;

namespace ContosoUniversity.Tests.Models
{
    public class PersonTests
    {
        [Fact]
        public void FullName_WithLastAndFirstName_ReturnsLastCommaFirst()
        {
            // Arrange
            var student = new Student { LastName = "Smith", FirstMidName = "John" };

            // Act
            var result = student.FullName;

            // Assert
            Assert.Equal("Smith, John", result);
        }

        [Fact]
        public void FullName_WithEmptyFirstName_ReturnsLastNameWithComma()
        {
            // Arrange
            var student = new Student { LastName = "Smith", FirstMidName = string.Empty };

            // Act
            var result = student.FullName;

            // Assert
            Assert.Equal("Smith, ", result);
        }

        [Fact]
        public void FullName_WithEmptyLastName_ReturnsCommaFirstName()
        {
            // Arrange
            var student = new Student { LastName = string.Empty, FirstMidName = "John" };

            // Act
            var result = student.FullName;

            // Assert
            Assert.Equal(", John", result);
        }

        [Fact]
        public void FullName_OnStudentInstance_ReturnsInheritedFullName()
        {
            // Arrange
            var student = new Student { LastName = "Doe", FirstMidName = "Jane" };

            // Act
            var result = student.FullName;

            // Assert
            Assert.Equal("Doe, Jane", result);
        }
    }
}
