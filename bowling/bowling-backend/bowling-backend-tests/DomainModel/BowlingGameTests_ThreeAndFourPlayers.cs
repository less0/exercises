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

    [Theory]
    [InlineData(0, 0, 0, 0, 0)]
    [InlineData(2, 1, 0, 0, 0)]
    [InlineData(8, 1, 1, 1, 1)]
    [InlineData(9, 2, 1, 1, 1)]
    [InlineData(12, 2, 2, 1, 1)]
    [InlineData(78, 10, 10, 10, 9)]
    [InlineData(80, 10, 10, 10, 10)]
    public void AddFrame_FourPlayers_FramesAreAddedCorrectly(int numberOfRolls,
        int expectedFramesPlayer1, 
        int expectedFramesPlayer2, 
        int expectedFramesPlayer3,
        int expectedFramesPlayer4)
    {
        var game = BowlingGame.StartNew(FourPlayers);

        AddNRolls(game, numberOfRolls);

        game.Frames[0].Should().HaveCount(expectedFramesPlayer1);
        game.Frames[1].Should().HaveCount(expectedFramesPlayer2);
        game.Frames[2].Should().HaveCount(expectedFramesPlayer3);
        game.Frames[3].Should().HaveCount(expectedFramesPlayer4);
    }

    [Theory]
    [InlineData(0, "Player 1")]
    [InlineData(1, "Player 1")]
    [InlineData(2, "Player 2")]
    [InlineData(7, "Player 4")]
    [InlineData(8, "Player 1")]
    [InlineData(80, "")]
    public void CurrentPlayer_FourPlayers_ReturnsCorrectName(int numberOfRolls, string expectedPlayerName)
    {
        var game = BowlingGame.StartNew(FourPlayers);

        AddNRolls(game, numberOfRolls);

        game.CurrentPlayer.Should().Be(expectedPlayerName);
    }  
}