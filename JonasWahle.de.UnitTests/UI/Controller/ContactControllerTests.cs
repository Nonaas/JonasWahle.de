using AwesomeAssertions;
using JonasWahle.de.Domain.Models;
using JonasWahle.de.UI.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace JonasWahle.de.UnitTests.UI.Controller
{
    public class ContactControllerTests
    {
        private readonly Mock<IOptions<SmtpSettings>> _mockSmtpOptions;
        private readonly Mock<ILogger<ContactController>> _mockLogger;
        private readonly ContactController _controller;
        private readonly SmtpSettings _smtpSettings;

        public ContactControllerTests()
        {
            _smtpSettings = new SmtpSettings
            {
                Host = "smtp.test.com",
                Port = 587,
                Username = "test@test.com",
                Password = "testpassword",
                FromAddress = "noreply@test.com",
                ToAddress = "contact@test.com"
            };

            _mockSmtpOptions = new Mock<IOptions<SmtpSettings>>();
            _mockSmtpOptions.Setup(x => x.Value).Returns(_smtpSettings);

            _mockLogger = new Mock<ILogger<ContactController>>();

            _controller = new ContactController(_mockSmtpOptions.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SendContactMessage_WithInvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidDto = new SendMessageDto
            {
                Name = null, // Invalid - required field
                Email = "john@example.com",
                Message = "This is a test message"
            };

            // Simulate invalid model state
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.SendContactMessage(invalidDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;

            // The Value should be a SerializableError (dictionary-like object)
            badRequestResult!.Value.Should().NotBeNull();

            // Cast to SerializableError to access the error details
            var serializableError = badRequestResult.Value as SerializableError;
            serializableError.Should().NotBeNull();
            serializableError.Should().ContainKey("Name");
            serializableError!["Name"].Should().BeEquivalentTo(new[] { "Name is required" });
        }

        [Theory]
        [InlineData("", "john@example.com", "Test message")]
        [InlineData("John", "invalid-email", "Test message")]
        [InlineData("John", "john@example.com", "")]
        public async Task SendContactMessage_WithInvalidData_ShouldHandleValidationErrors(
            string name, string email, string message)
        {
            // Arrange
            var dto = new SendMessageDto
            {
                Name = name,
                Email = email,
                Message = message
            };

            // Simulate model validation by adding errors for invalid data
            if (string.IsNullOrEmpty(name))
                _controller.ModelState.AddModelError("Name", "Name is required");

            if (email == "invalid-email")
                _controller.ModelState.AddModelError("Email", "Invalid email format");

            if (string.IsNullOrEmpty(message))
                _controller.ModelState.AddModelError("Message", "Message is required");

            // Act
            var result = await _controller.SendContactMessage(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Act & Assert
            _controller.Should().NotBeNull();
        }

        [Fact]
        public void SmtpSettings_ShouldBeConfiguredCorrectly()
        {
            // Assert
            _mockSmtpOptions.Object.Value.Host.Should().Be("smtp.test.com");
            _mockSmtpOptions.Object.Value.Port.Should().Be(587);
            _mockSmtpOptions.Object.Value.FromAddress.Should().Be("noreply@test.com");
            _mockSmtpOptions.Object.Value.ToAddress.Should().Be("contact@test.com");
        }
    }
}
