using bowling_backend_core.DomainModel;
using FluentAssertions;

namespace bowling_backend_tests.DomainModel;

public partial class FrameTests
{
    [Fact]
    public void LastFrame_ExceedsMaximumPinsPerRoll_ThrowsArgumentException()
    {
        var exception = Record.Exception(() => Frame.LastFrame(pinsWithFirstRoll: 11, cumulativeScore: 0));
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
        var frame = Frame.LastFrame(firstRollPins, cumulativeScore: 0);
        frame.AddRoll(secondRollPins);
        frame.AddRoll(0);
    }

    [Fact]
    public void AddFrame_IsLastFrame_ThreeRollsAreNotAllowed()
    {
        var frame = Frame.LastFrame(pinsWithFirstRoll: 0, cumulativeScore: 0);
        frame.AddRoll(0);
        var exception = Record.Exception(() => frame.AddRoll(0));
        exception.Should().BeOfType<InvalidOperationException>();
    }
}