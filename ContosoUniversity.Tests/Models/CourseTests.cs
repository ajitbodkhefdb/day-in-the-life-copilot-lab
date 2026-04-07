using ContosoUniversity.Core.Models;
using Xunit;

namespace ContosoUniversity.Tests.Models
{
    public class CourseTests
    {
        [Fact]
        public void MaxEnrollment_Default_IsThirty()
        {
            // Arrange & Act
            var course = new Course();

            // Assert
            Assert.Equal(30, course.MaxEnrollment);
        }

        [Fact]
        public void MaxEnrollment_WhenSet_ReturnsAssignedValue()
        {
            // Arrange
            var course = new Course { MaxEnrollment = 50 };

            // Act & Assert
            Assert.Equal(50, course.MaxEnrollment);
        }
    }
}
