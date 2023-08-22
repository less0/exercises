using bowling_backend_core.DomainModel;
using FluentAssertions;

namespace bowling_backend_tests.DomainModel;

public partial class FrameTests
{
    [Fact]
    public void LastFrame_ExceedsMaximumPinsPerRoll_ThrowsArgumentException()
    {
        var exception = Record.Exception(() => Frame.CreateLastFrame(pinsWithFirstRoll: 11));
        exception.Should().BeOfType<ArgumentException>();
    }

    [Theory]
    [InlineData(10, 0)]
    [InlineData(9, 1)]
    [InlineData(1, 9)]
    [InlineData(0, 10)]
    [InlineData(10, 10)] // Also two strikes are possible!
    public void AddFrame_IsLastFrame_ThreeRollsAllowedOnSpareOrStrike(int firstRollPins, int secondRollPins)
    {
        var frame = Frame.CreateLastFrame(firstRollPins);
        frame.AddRoll(secondRollPins);
        frame.AddRoll(0);
    }

    [Fact]
    public void AddFrame_IsLastFrame_ThreeRollsAreNotAllowed()
    {
        var frame = Frame.CreateLastFrame(pinsWithFirstRoll: 0);
        frame.AddRoll(0);
        var exception = Record.Exception(() => frame.AddRoll(0));
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void SetBonusPoints_IsLastFrame_ThrowsInvalidOperationException()
    {
        var frame = Frame.CreateLastFrame(0);
        frame.AddRoll(0);
        var exception = Record.Exception(() => frame.AddBonusPoints(Frame.CreateFrame(0)));
        exception.Should().BeOfType<InvalidOperationException>();
    }
}