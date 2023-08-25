using System.ComponentModel.DataAnnotations;
using bowling_backend_core.DomainModel;
using FluentAssertions;

namespace bowling_backend_tests.DomainModel;

public partial class FrameTests
{
    [Fact]
    public void CreateFrame_IdShouldNotBeEmpty()
    {
        var frame = Frame.CreateFrame(0);
        frame.Id.Should().NotBeEmpty();
    }  

    [Fact]
    public void CreateFrame_RollsAreInitializedCorrectly()
    {
        var pins = Random.Shared.Next(0, 11);

        var frame = Frame.CreateFrame(pins);

        frame.Rolls.Should().HaveCount(1);
        frame.Rolls.Should().HaveElementAt(0, pins);
    }

    [Fact]
    public void CreateFrame_ExceedsMaximumPinsPerRoll_ThrowsArgumentException()
    {
        var exception = Record.Exception(() => Frame.CreateFrame(11));
        exception.Should().BeOfType<ArgumentException>();
    }

    [Fact]
    public void AddRoll_NegativePins_ThrowsArgumentException()
    {
        var frame = Frame.CreateFrame(1);
        var exception = Record.Exception(() => frame.AddRoll(-1));
        exception.Should().BeOfType<ArgumentException>();
    }

    [Fact]
    public void CreateFrame_NegativePins_ThrowsArgumentException()
    {
        var exception = Record.Exception(() => Frame.CreateFrame(-1));
        exception.Should().BeOfType<ArgumentException>();
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    [InlineData(1, 9)]
    [InlineData(2, 8)]
    [InlineData(3, 7)]
    [InlineData(7, 3)]
    [InlineData(8, 2)]
    [InlineData(9, 1)]
    public void AddRoll_IsValid_RollIsAdded(int firstRollPins, int secondRollPins)
    {
        var frame = Frame.CreateFrame(firstRollPins);
        frame.AddRoll(secondRollPins);
        frame.Rolls.Should().HaveCount(2);
        frame.Rolls.Should().HaveElementAt(1, secondRollPins);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 9)]
    [InlineData(3, 8)]
    [InlineData(4, 7)]
    [InlineData(5, 6)]
    [InlineData(6, 5)]
    [InlineData(7, 4)]
    [InlineData(8, 3)]
    [InlineData(9, 2)]
    [InlineData(10, 1)]
    public void AddRoll_IsInvalid_ThrowsInvalidOperationException(int firstRollPins, int secondRollPins)
    {
        var frame = Frame.CreateFrame(firstRollPins);
        var exception = Record.Exception(() => frame.AddRoll(secondRollPins));
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void AddRoll_ThirdRoll_ThrowsInvalidOperationException()
    {
        var frame = Frame.CreateFrame(0);
        frame.AddRoll(0);
        var exception = Record.Exception(() => frame.AddRoll(0));
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void AddRoll_ExceedsMaximumPinsPerRoll_ThrowsArgumentException()
    {
        var frame = Frame.CreateFrame(0);
        var exception = Record.Exception(() => frame.AddRoll(11));
        exception.Should().BeOfType<ArgumentException>();
    }

    [Fact]
    public void IsFinished_TwoRolls_ReturnsTrue()
    {
        var frame = Frame.CreateFrame(0);
        frame.AddRoll(0);
        frame.IsFinished.Should().BeTrue();
    }

    [Fact]
    public void IsFinished_Strike_ReturnsTrue()
    {
        var frame = Frame.CreateFrame(10);
        frame.IsFinished.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(9)]
    public void IsFinished_OneRollWasMadeWithoutStrike_ReturnsFalse(int pins)
    {
        var frame = Frame.CreateFrame(pins);
        frame.IsFinished.Should().BeFalse();
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 1, 1)]
    [InlineData(0, 9, 9)]
    [InlineData(0, 10, 10)]
    [InlineData(9, 0, 9)]
    [InlineData(1, 0, 1)]
    [InlineData(7, 3, 10)]
    [InlineData(3, 4, 7)]
    public void Score_IsCalculatedCorrectly(int firstRollPins, int secondRollPins, int expectedScore)
    {
        var frame = Frame.CreateFrame(firstRollPins);
        frame.AddRoll(secondRollPins);
        frame.Score.Should().Be(expectedScore);
    }

    [Fact]
    public void Score_Strike_IsCalculatedCorrectlyForStartFrame()
    {
        var frame = Frame.CreateFrame(10);
        frame.Score.Should().Be(10);
    }
}