namespace MealPlanner.Application.Tests;

public sealed class StringExtensionsTests
{
    [Theory]
    [InlineData("Banana", "banana")]
    [InlineData("Crème Brûlée", "creme brulee")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    public void ToNormalizedString_NormalizesValue(string input, string expected)
        => Assert.Equal(expected, input.ToNormalizedString());

    [Fact]
    public void ToNormalizedString_WhenNull_ReturnsEmpty()
        => Assert.Equal(string.Empty, ((string)null!).ToNormalizedString());
}
