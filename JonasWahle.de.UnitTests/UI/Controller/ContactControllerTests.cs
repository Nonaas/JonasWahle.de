using AwesomeAssertions;
using JonasWahle.de.Data.Models;
using JonasWahle.de.Domain.Interfaces;
using JonasWahle.de.Domain.Models;
using JonasWahle.de.UI.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace JonasWahle.de.UnitTests.UI.Controller
{
    public class ContactControllerTests
    {
        private readonly Mock<ISmtpSettingService> _mockSmtpService;
        private readonly Mock<ILogger<ContactController>> _mockLogger;
        private readonly ContactController _controller;
        private readonly SmtpSetting _smtpSettings;

        public ContactControllerTests()
        {
            _smtpSettings = new SmtpSetting
            {
                Host = "smtp.test.com",
                Port = 587,
                Username = "test@test.com",
                Password = "testpassword",
                FromAddress = "noreply@test.com",
                ToAddress = "contact@test.com"
            };

            _mockSmtpService = new Mock<ISmtpSettingService>();
            _mockSmtpService.Setup(x => x.GetSmtpSettingAsync()).ReturnsAsync(_smtpSettings);

            _mockLogger = new Mock<ILogger<ContactController>>();

            _controller = new ContactController(_mockLogger.Object, _mockSmtpService.Object);
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
        public async Task SmtpSettings_ShouldBeConfiguredCorrectlyAsync()
        {
            SmtpSetting? smtpSetting = await _mockSmtpService.Object.GetSmtpSettingAsync();

            // Assert
            smtpSetting.Should().NotBeNull();
            smtpSetting.Host.Should().Be("smtp.test.com");
            smtpSetting.Port.Should().Be(587);
            smtpSetting.FromAddress.Should().Be("noreply@test.com");
            smtpSetting.ToAddress.Should().Be("contact@test.com");
        }
    }
}
