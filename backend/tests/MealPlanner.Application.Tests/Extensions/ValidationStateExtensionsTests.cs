using DannyGoodacre.Primitives;
using NUnit.Framework;

namespace MealPlanner.Application.Tests.Extensions;

[TestFixture]
public sealed class ValidationStateExtensionsTests
{
    private const string TestFieldName = "Test Field Name";

    private enum TestEnum
    {
        First = 1,

        Second = 2
    }

    [Test]
    public void IsLessThanOrEqualTo_WhenLessThanOrEqualTo_ShouldReturnTrue()
    {
        // Arrange
        ValidationState state = new();

        const decimal value = 123.45m;

        const decimal upperLimit = 500m;

        // Act
        bool result = state.IsLessThanOrEqualTo(value, TestFieldName, upperLimit);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsTrue(result);

            Assert.IsFalse(state.HasErrors);

            Assert.IsEmpty(state.Errors);
        }
    }

    [Test]
    public void IsLessThanOrEqualTo_WhenGreaterThan_ShouldReturnFalse()
    {
        // Arrange
        ValidationState state = new();

        const decimal value = 123.45m;

        const decimal upperLimit = 100m;

        // Act
        bool result = state.IsLessThanOrEqualTo(value, TestFieldName, upperLimit);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.IsNotEmpty(state.Errors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            Assert.That(state.Errors.Keys, Does.Contain(TestFieldName));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must be less than or equal to 100."));
        }
    }

    [Test]
    public void IsLessThanOrEqualTo_WhenGreaterThanWithUpperLimitName_ShouldReturnFalse()
    {
        // Arrange
        ValidationState state = new();

        const decimal value = 123.45m;

        const decimal upperLimit = 100m;

        const string upperLimitName = "Test Upper Limit Name";

        // Act
        bool result = state.IsLessThanOrEqualTo(value, TestFieldName, upperLimit, upperLimitName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.IsNotEmpty(state.Errors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            Assert.That(state.Errors.Keys, Does.Contain(TestFieldName));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must be less than or equal to Test Upper Limit Name."));
        }
    }

    [Test]
    public void IsMinimumLength_WhenLengthIsGreaterThanOrEqualToMinimum_ShouldReturnTrue()
    {
        // Arrange
        ValidationState state = new();

        const string value = "Hello World";

        const int minimumLength = 5;

        // Act
        bool result = state.IsMinimumLength(value, minimumLength, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsTrue(result);

            Assert.IsFalse(state.HasErrors);

            Assert.IsEmpty(state.Errors);
        }
    }

    [Test]
    public void IsMinimumLength_WhenLengthIsLessThanMinimum_ShouldReturnFalse()
    {
        // Arrange
        ValidationState state = new();

        const string value = "Hi";

        const int minimumLength = 5;

        // Act
        bool result = state.IsMinimumLength(value, minimumLength, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must be at least 5 characters long."));
        }
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void IsMinimumLength_WhenValueIsNullOrEmptyOrWhitespace_ShouldReturnFalse(string? value)
    {
        // Arrange
        ValidationState state = new();

        const int minimumLength = 5;

        // Act
        bool result = state.IsMinimumLength(value!, minimumLength, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must not be null, empty, or whitespace."));
        }
    }

    [Test]
    public void IsNotEmpty_WhenListContainsItems_ShouldReturnTrue()
    {
        // Arrange
        ValidationState state = new();

        List<int> value = [1, 2, 3];

        // Act
        bool result = state.IsNotEmpty(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsTrue(result);

            Assert.IsFalse(state.HasErrors);

            Assert.IsEmpty(state.Errors);
        }
    }

    [Test]
    public void IsNotEmpty_WhenListIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        ValidationState state = new();

        List<int> value = [];

        // Act
        bool result = state.IsNotEmpty(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must not be empty."));
        }
    }

    [Test]
    public void IsEmpty_WhenListIsEmpty_ShouldReturnTrue()
    {
        // Arrange
        ValidationState state = new();
        List<int> value = [];

        // Act
        bool result = state.IsEmpty(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsTrue(result);

            Assert.IsFalse(state.HasErrors);

            Assert.IsEmpty(state.Errors);
        }
    }

    [Test]
    public void IsEmpty_WhenListContainsItems_ShouldReturnFalse()
    {
        // Arrange
        ValidationState state = new();

        List<int> value = [1];

        // Act
        bool result = state.IsEmpty(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must be empty."));
        }
    }

    [Test]
    public void IsNonNegative_WhenValueIsZeroOrPositive_ShouldReturnTrue()
    {
        // Arrange
        ValidationState state = new();
        const decimal value = 0m;

        // Act
        bool result = state.IsNonNegative(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsTrue(result);

            Assert.IsFalse(state.HasErrors);

            Assert.IsEmpty(state.Errors);
        }
    }

    [Test]
    public void IsNonNegative_WhenValueIsNegative_ShouldReturnFalse()
    {
        // Arrange
        ValidationState state = new();
        const decimal value = -1.5m;

        // Act
        bool result = state.IsNonNegative(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must be greater than or equal to 0."));
        }
    }

    [Test]
    public void IsNotNullEmptyOrWhitespace_WhenValueHasContent_ShouldReturnTrue()
    {
        // Arrange
        ValidationState state = new();
        const string value = "Valid Content";

        // Act
        bool result = state.IsNotNullEmptyOrWhitespace(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsTrue(result);

            Assert.IsFalse(state.HasErrors);

            Assert.IsEmpty(state.Errors);
        }
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void IsNotNullEmptyOrWhitespace_WhenValueIsInvalid_ShouldReturnFalse(string? value)
    {
        // Arrange
        ValidationState state = new();

        // Act
        bool result = state.IsNotNullEmptyOrWhitespace(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must not be null, empty, or whitespace."));
        }
    }

    [Test]
    public void IsNull_WhenValueIsNull_ShouldReturnTrue()
    {
        // Arrange
        ValidationState state = new();
        object? value = null;

        // Act
        bool result = state.IsNull(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsTrue(result);

            Assert.IsFalse(state.HasErrors);

            Assert.IsEmpty(state.Errors);
        }
    }

    [Test]
    public void IsNull_WhenValueIsNotNull_ShouldReturnFalse()
    {
        // Arrange
        ValidationState state = new();
        object value = new();

        // Act
        bool result = state.IsNull(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must be null."));
        }
    }

    [Test]
    public void IsNotNull_WhenValueIsNotNull_ShouldReturnTrue()
    {
        // Arrange
        ValidationState state = new();
        object value = new();

        // Act
        bool result = state.IsNotNull(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsTrue(result);

            Assert.IsFalse(state.HasErrors);

            Assert.IsEmpty(state.Errors);
        }
    }

    [Test]
    public void IsNotNull_WhenValueIsNull_ShouldReturnFalse()
    {
        // Arrange
        ValidationState state = new();
        object? value = null;

        // Act
        bool result = state.IsNotNull(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must not be null."));
        }
    }

    [Test]
    public void IsPositive_WhenValueIsPositive_ShouldReturnTrue()
    {
        // Arrange
        ValidationState state = new();
        const decimal value = 0.01m;

        // Act
        bool result = state.IsPositive(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsTrue(result);

            Assert.IsFalse(state.HasErrors);

            Assert.IsEmpty(state.Errors);
        }
    }

    [TestCase(0)]
    [TestCase(-5.5)]
    public void IsPositive_WhenValueIsNonPositive_ShouldReturnFalse(decimal value)
    {
        // Arrange
        ValidationState state = new();

        // Act
        bool result = state.IsPositive(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must be greater than 0."));
        }
    }

    [Test]
    public void IsNonEmptyGuid_WhenGuidIsNotEmpty_ShouldReturnTrue()
    {
        // Arrange
        ValidationState state = new();

        Guid value = Guid.NewGuid();

        // Act
        bool result = state.IsNonEmptyGuid(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsTrue(result);

            Assert.IsFalse(state.HasErrors);

            Assert.IsEmpty(state.Errors);
        }
    }

    [Test]
    public void IsNonEmptyGuid_WhenGuidIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        ValidationState state = new();

        Guid value = Guid.Empty;

        // Act
        bool result = state.IsNonEmptyGuid(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must not be empty."));
        }
    }


    [Test]
    public void IsValidEnum_WhenValueIsDefined_ShouldReturnTrue()
    {
        // Arrange
        ValidationState state = new();

        const TestEnum value = TestEnum.Second;

        // Act
        bool result = state.IsValidEnum(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsTrue(result);

            Assert.IsFalse(state.HasErrors);

            Assert.IsEmpty(state.Errors);
        }
    }

    [Test]
    public void IsValidEnum_WhenValueIsNotDefined_ShouldReturnFalse()
    {
        // Arrange
        ValidationState state = new();

        const TestEnum value = (TestEnum)99;

        // Act
        bool result = state.IsValidEnum(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must be defined."));
        }
    }

    [TestCase("http://example.com")]
    [TestCase("https://example.com/path?query=1")]
    public void IsValidUrl_WhenUrlIsValid_ShouldReturnTrue(string value)
    {
        // Arrange
        ValidationState state = new();

        // Act
        bool result = state.IsValidUrl(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsTrue(result);

            Assert.IsFalse(state.HasErrors);

            Assert.IsEmpty(state.Errors);
        }
    }

    [TestCase("ftp://example.com")]
    [TestCase("test-string")]
    [TestCase("http://")]
    public void IsValidUrl_WhenUrlIsInvalid_ShouldReturnFalse(string value)
    {
        // Arrange
        ValidationState state = new();

        // Act
        bool result = state.IsValidUrl(value, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must be a valid URL."));
        }
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void IsValidUrl_WhenValueIsNullOrEmptyOrWhitespace_ShouldReturnFalse(string? value)
    {
        // Arrange
        ValidationState state = new();

        // Act
        bool result = state.IsValidUrl(value!, TestFieldName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.IsFalse(result);

            Assert.IsTrue(state.HasErrors);

            Assert.That(state.Errors, Has.Count.EqualTo(1));

            List<string> errors = state.Errors[TestFieldName].ToList();

            Assert.That(errors[0], Is.EqualTo("Must not be null, empty, or whitespace."));
        }
    }
}
