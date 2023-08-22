using System.ComponentModel.DataAnnotations;
using bowling_backend_core.DomainModel;
using FluentAssertions;

namespace bowling_backend_tests.DomainModel;

public partial class FrameTests
{
    [Fact]
    public void StartFrame_IdShouldNotBeEmpty()
    {
        var frame = Frame.FirstFrame(0);
        frame.Id.Should().NotBeEmpty();
    }  

    [Fact]
    public void StartFrame_RollsAreInitializedCorrectly()
    {
        var pins = Random.Shared.Next(0, 11);

        var frame = Frame.FirstFrame(pins);

        frame.Rolls.Should().HaveCount(1);
        frame.Rolls.Should().HaveElementAt(0, pins);
    }

    [Fact]
    public void FirstFrame_ExceedsMaximumPinsPerRoll_ThrowsArgumentException()
    {
        var exception = Record.Exception(() => Frame.FirstFrame(11));
        exception.Should().BeOfType<ArgumentException>();
    }

    [Fact]
    public void MidgameFrame_ExceedsMaximumPinsPerRoll_ThrowsArgumentException()
    {
        var exception = Record.Exception(() => Frame.MidgameFrame(pinsWithFirstRoll: 11, cumulativeScore: 0));
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
        var frame = Frame.FirstFrame(firstRollPins);
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
        var frame = Frame.FirstFrame(firstRollPins);
        var exception = Record.Exception(() => frame.AddRoll(secondRollPins));
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void AddRoll_ThirdRoll_ThrowsInvalidOperationException()
    {
        var frame = Frame.FirstFrame(0);
        frame.AddRoll(0);
        var exception = Record.Exception(() => frame.AddRoll(0));
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void AddRoll_IsFirstFrame_ExceedsMaximumPinsPerRoll_ThrowsArgumentException()
    {
        var frame = Frame.FirstFrame(0);
        var exception = Record.Exception(() => frame.AddRoll(11));
        exception.Should().BeOfType<ArgumentException>();
    }

    [Fact]
    public void AddRoll_IsMidgameFrame_ExceedsMaximumPinsPerRoll_ThrowsArgumentException()
    {
        var frame = Frame.MidgameFrame(pinsWithFirstRoll: 0, cumulativeScore: 0);
        var exception = Record.Exception(() => frame.AddRoll(11));
        exception.Should().BeOfType<ArgumentException>();
    }

    [Fact]
    public void IsFinished_TwoRolls_ReturnsTrue()
    {
        var frame = Frame.FirstFrame(0);
        frame.AddRoll(0);
        frame.IsFinished.Should().BeTrue();
    }

    [Fact]
    public void IsFinished_Strike_ReturnsTrue()
    {
        var frame = Frame.FirstFrame(10);
        frame.IsFinished.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(9)]
    public void IsFinished_OneRollWasMadeWithoutStrike_ReturnsFalse(int pins)
    {
        var frame = Frame.FirstFrame(pins);
        frame.IsFinished.Should().BeFalse();
    }

    [Fact]
    public void IsStrike_Strike_ReturnsTrue()
    {
        var frame = Frame.FirstFrame(10);
        frame.IsStrike.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(9)]
    public void IsStrike_OneRollWasMadeWithoutAStrike_ReturnsFalse(int pins)
    {
        var frame = Frame.FirstFrame(pins);
        frame.IsStrike.Should().BeFalse();
    }

    [Theory]
    [InlineData(0, 0, false)]
    [InlineData(0, 9, false)]
    [InlineData(1, 8, false)]
    [InlineData(5, 4, false)]
    [InlineData(8, 1, false)]
    [InlineData(9, 0, false)]
    [InlineData(0, 10, true)]
    [InlineData(1, 9, true)]
    [InlineData(5, 5, true)]
    [InlineData(9, 1, true)]
    public void IsSpare_ReturnsExpectedValue(int firstRollPins, int secondRollPins, bool expectedValue)
    {
        var frame = Frame.FirstFrame(firstRollPins);
        frame.AddRoll(secondRollPins);
        frame.IsSpare.Should().Be(expectedValue);
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
    public void Score_IsCalculatedCorrectlyForStartFrame(int firstRollPins, int secondRollPins, int expectedScore)
    {
        var frame = Frame.FirstFrame(firstRollPins);
        frame.AddRoll(secondRollPins);
        frame.Score.Should().Be(expectedScore);
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(0, 3, 3, 6)]
    [InlineData(0, 3, 7, 10)]
    [InlineData(1, 3, 7, 11)]
    [InlineData(10, 3, 7, 20)]
    public void Score_IsCalculatedCorrectlyForMidgameFrame(int cumulativeScore, int firstRollPins, int secondRollPins, int expectedScore)
    {
        var frame = Frame.MidgameFrame(firstRollPins, cumulativeScore);
        frame.AddRoll(secondRollPins);
        frame.Score.Should().Be(expectedScore);
    }

    [Fact]
    public void Score_Strike_IsCalculatedCorrectlyForStartFrame()
    {
        var frame = Frame.FirstFrame(10);
        frame.Score.Should().Be(10);
    }
}