using bowling_backend_core.DomainModel;
using FluentAssertions;

namespace bowling_backend_tests.DomainModel;

public partial class FrameTests
{
    [Fact]
    public void AddBonusPoints_IsNotFinished_ThrowsInvalidOperationException()
    {
        var frame = Frame.CreateFrame(0);

        var nextFrame = Frame.CreateFrame(0);
        nextFrame.AddRoll(0);

        var exception = Record.Exception(() => frame.AddBonusPoints(Frame.CreateFrame(0)));
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void AddBonusPoints_NextFrameIsNotFinished_ThrowsArgumentException()
    {
        var frame = Frame.CreateFrame(0);
        frame.AddRoll(0);

        var nextFrame = Frame.CreateFrame(0);

        var exception = Record.Exception(() => frame.AddBonusPoints(nextFrame));
        exception.Should().BeOfType<ArgumentException>();
    }

    [Fact, Repeat(10)]
    public void AddBonusPoints_IsNoSpareOrStrike_NoBonusIsAdded()
    {
        // random rolls, but ensuring the first frame is not a spare
        var firstFrameFirstRoll = Random.Shared.Next(10);
        var firstFrameSecondRoll = Random.Shared.Next(10 - firstFrameFirstRoll);

        var secondFrameFirstRoll = Random.Shared.Next(10);
        var secondFrameSecondRoll = Random.Shared.Next(10 - secondFrameFirstRoll + 1); // allow spares for the second frame

        var frame = Frame.CreateFrame(firstFrameFirstRoll);
        frame.AddRoll(firstFrameSecondRoll);

        var nextFrame = Frame.CreateFrame(secondFrameFirstRoll);
        nextFrame.AddRoll(secondFrameSecondRoll);

        frame.AddBonusPoints(nextFrame);

        frame.Score.Should().Be(firstFrameFirstRoll + firstFrameSecondRoll);
    }

    [Fact, Repeat(10)]
    public void AddBonusPoints_Spare_FirstRollIsAdded()
    {
        var firstFrameFirstRoll = Random.Shared.Next(10);
        var firstFrameSecondRoll = 10 - firstFrameFirstRoll;

        var secondFrameFirstRoll = Random.Shared.Next(10);
        var secondFrameSecondRoll = Random.Shared.Next(10 - secondFrameFirstRoll + 1); 

        var frame = Frame.CreateFrame(firstFrameFirstRoll);
        frame.AddRoll(firstFrameSecondRoll);

        var nextFrame = Frame.CreateFrame(secondFrameFirstRoll);
        nextFrame.AddRoll(secondFrameSecondRoll);

        frame.AddBonusPoints(nextFrame);

        frame.Score.Should().Be(firstFrameFirstRoll + firstFrameSecondRoll + secondFrameFirstRoll);
    }

    [Fact, Repeat(10)]
    public void AddBonusPoints_Strike_NextTwoRollsAreAdded()
    {
        var secondFrameFirstRoll = Random.Shared.Next(10);
        var secondFrameSecondRoll = Random.Shared.Next(10 - secondFrameFirstRoll + 1); 

        var frame = Frame.CreateFrame(10);

        var nextFrame = Frame.CreateFrame(secondFrameFirstRoll);
        nextFrame.AddRoll(secondFrameSecondRoll);

        frame.AddBonusPoints(nextFrame);

        frame.Score.Should().Be(10 + secondFrameFirstRoll + secondFrameSecondRoll);
    }

    [Fact]
    public void AddBonusPoints_Strike_OnlyNextTwoRollsAreAddedIfNextFrameIsLastFrame()
    {
        var frame = Frame.CreateFrame(10);
        var nextFrame = Frame.CreateLastFrame(5);
        nextFrame.AddRoll(5);
        nextFrame.AddRoll(5);

        frame.AddBonusPoints(nextFrame);

        frame.Score.Should().Be(20);
        
    }
}