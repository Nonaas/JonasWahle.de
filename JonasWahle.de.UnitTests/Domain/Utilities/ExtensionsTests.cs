using AwesomeAssertions;
using JonasWahle.de.Domain.Utilities;
using Xunit;

namespace JonasWahle.de.UnitTests.Domain.Utilities
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData("test", "Test")]
        [InlineData("TEST", "TEST")]
        [InlineData("t", "T")]
        [InlineData("", "")]
        [InlineData("myProperty", "MyProperty")]
        public void ToPascalCase_ShouldConvertStringCorrectly(string input, string expected)
        {
            // Act
            var result = Extensions.ToPascalCase(input);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void ToPascalCase_WithNullInput_ShouldReturnNull()
        {
            // Act
            var result = Extensions.ToPascalCase(null!);

            // Assert
            result.Should().BeNull();
        }
    }
}
