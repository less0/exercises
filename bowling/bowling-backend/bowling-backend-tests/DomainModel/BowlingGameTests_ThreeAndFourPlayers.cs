using bowling_backend_core.DomainModel;
using FluentAssertions;

namespace bowling_backend_tests.DomainModel;

public partial class BowlingGameTests
{
    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(6, 1, 1, 1)]
    [InlineData(7, 2, 1, 1)]
    [InlineData(60, 10, 10, 10)]
    public void AddFrame_ThreePlayers_FramesAreAddedCorrectly(int numberOfRolls, 
        int expectedFramesPlayer1, 
        int expectedFramesPlayer2, 
        int expectedFramesPlayer3)
    {
        var game = BowlingGame.StartNew(ThreePlayers);

        AddNRolls(game, numberOfRolls);

        game.Frames[0].Should().HaveCount(expectedFramesPlayer1);
        game.Frames[1].Should().HaveCount(expectedFramesPlayer2);
        game.Frames[2].Should().HaveCount(expectedFramesPlayer3);
    }
}