using bowling_backend_core.DomainModel;
using FluentAssertions;
using NuGet.Frameworks;

namespace bowling_backend_tests;

public class FrameTests
{
    [Fact]
    public void StartFrame_IdShouldNotBeEmpty()
    {
        var frame = Frame.StartFrame(0);
        frame.Id.Should().NotBeEmpty();
    }  

    [Fact]
    public void StartFrame_RollsAreInitializedCorrectly()
    {
        var pins = Random.Shared.Next(0, 11);

        var frame = Frame.StartFrame(pins);

        frame.Rolls.Should().HaveCount(1);
        frame.Rolls.Should().HaveElementAt(0, pins);
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
        var frame = Frame.StartFrame(firstRollPins);
        frame.AddRoll(secondRollPins);
        frame.Rolls.Should().HaveCount(2);
        frame.Rolls.Should().HaveElementAt(1, secondRollPins);
    }

    [Theory]
    [InlineData(0, 11)]
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
        var frame = Frame.StartFrame(firstRollPins);
        var exception = Record.Exception(() => frame.AddRoll(secondRollPins));
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void AddRoll_ThirdRoll_ThrowsInvalidOperationException()
    {
        var frame = Frame.StartFrame(0);
        frame.AddRoll(0);
        var exception = Record.Exception(() => frame.AddRoll(0));
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void IsFinished_TwoRolls_ReturnsTrue()
    {
        var frame = Frame.StartFrame(0);
        frame.AddRoll(0);
        frame.IsFinished.Should().BeTrue();
    }

    [Fact]
    public void IsFinished_Strike_ReturnsTrue()
    {
        var frame = Frame.StartFrame(10);
        frame.IsFinished.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(9)]
    public void IsFinished_OneRollWasMadeWithoutStrike_ReturnsFalse(int pins)
    {
        var frame = Frame.StartFrame(pins);
        frame.IsFinished.Should().BeFalse();
    }

    [Fact]
    public void IsStrike_Strike_ReturnsTrue()
    {
        var frame = Frame.StartFrame(10);
        frame.IsStrike.Should().BeTrue();
    }
}