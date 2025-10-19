using Microsoft.AspNetCore.Components;
using Moq;
using MudBlazor;
using Xunit;

namespace JonasWahle.de.UnitTests.UI.Services
{
    public class SnackbarServiceTests
    {
        private readonly Mock<ISnackbar> _mockSnackbar;
        private readonly JonasWahle.de.UI.Services.SnackbarService _snackbarService;

        public SnackbarServiceTests()
        {
            _mockSnackbar = new Mock<ISnackbar>();
            _snackbarService = new JonasWahle.de.UI.Services.SnackbarService(_mockSnackbar.Object);
        }

        [Fact]
        public void ShowSuccess_ShouldCallSnackbarWithCorrectParameters()
        {
            // Arrange
            const string message = "Success message";

            // Act
            _snackbarService.ShowSuccess(message);

            // Assert
            _mockSnackbar.Verify(x => x.Add(
                It.IsAny<MarkupString>(),
                Severity.Success,
                It.IsAny<Action<SnackbarOptions>>(),
                It.IsAny<string>()
            ), Times.Once);
        }

        [Fact]
        public void ShowError_ShouldCallSnackbarWithCorrectParameters()
        {
            // Arrange
            const string message = "Error message";

            // Act
            _snackbarService.ShowError(message);

            // Assert
            _mockSnackbar.Verify(x => x.Add(
                It.IsAny<MarkupString>(),
                Severity.Error,
                It.IsAny<Action<SnackbarOptions>>(),
                It.IsAny<string>()
            ), Times.Once);
        }

        [Fact]
        public void ShowInfo_ShouldCallSnackbarWithCorrectParameters()
        {
            // Arrange
            const string message = "Info message";

            // Act
            _snackbarService.ShowInfo(message);

            // Assert
            _mockSnackbar.Verify(x => x.Add(
                It.IsAny<MarkupString>(),
                Severity.Info,
                It.IsAny<Action<SnackbarOptions>>(),
                It.IsAny<string>()
            ), Times.Once);
        }

        [Fact]
        public void ShowWarning_ShouldCallSnackbarWithCorrectParameters()
        {
            // Arrange
            const string message = "Warning message";

            // Act
            _snackbarService.ShowWarning(message);

            // Assert
            _mockSnackbar.Verify(x => x.Add(
                It.IsAny<MarkupString>(),
                Severity.Warning,
                It.IsAny<Action<SnackbarOptions>>(),
                It.IsAny<string>()
            ), Times.Once);
        }
    }
}
