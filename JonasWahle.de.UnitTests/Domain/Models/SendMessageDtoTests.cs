using AwesomeAssertions;
using JonasWahle.de.Domain.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace JonasWahle.de.UnitTests.Domain.Models
{
    public class SendMessageDtoTests
    {
        [Fact]
        public void SendMessageDto_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var dto = new SendMessageDto
            {
                Name = "John Doe",
                Email = "john@example.com",
                Message = "This is a test message"
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Theory]
        [InlineData("", "john@example.com", "message")]
        [InlineData("A", "john@example.com", "message")]
        [InlineData(null, "john@example.com", "message")]
        public void SendMessageDto_WithInvalidName_ShouldFailValidation(string name, string email, string message)
        {
            // Arrange
            var dto = new SendMessageDto { Name = name, Email = email, Message = message };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            validationResults.Should().NotBeEmpty();
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("Name"));
        }

        [Theory]
        [InlineData("John", "invalid-email", "message")]
        [InlineData("John", "", "message")]
        [InlineData("John", null, "message")]
        public void SendMessageDto_WithInvalidEmail_ShouldFailValidation(string name, string email, string message)
        {
            // Arrange
            var dto = new SendMessageDto { Name = name, Email = email, Message = message };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            validationResults.Should().NotBeEmpty();
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("Email"));
        }

        private static List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
    }
}
