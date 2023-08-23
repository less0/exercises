using bowling_backend_core.DomainModel;
using FluentAssertions;

namespace bowling_backend_tests;

public class BowlingGameTests
{
    [Theory]
    [InlineData("Peter", "Paul", "Mary")]
    public void StartNew_PlayerNamesAreSet(params string[] playerNames)
    {
        var game = BowlingGame.StartNew(playerNames);
        game.PlayerNames.Should().BeEquivalentTo(playerNames);
    }

    [Fact]
    public void StartNew_ThrowsWhenZeroPlayers()
    {
        var exception = Record.Exception(() => BowlingGame.StartNew(Array.Empty<string>()));
        exception.Should().BeOfType<ArgumentException>();
    }

    [Fact]
    public void StartNew_IdIsSet()
    {
        var game = BowlingGame.StartNew(new[] {"Player"});
        game.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("A")]
    [InlineData("A", "B")]
    [InlineData("Tick", "Trick", "Tuck")]
    public void StartNew_FramesHasRowsAccordingToPlayers(params string[] players)
    {
        var game = BowlingGame.StartNew(players);
        game.Frames.Should().HaveSameCount(players);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(2, 0, 0, 0)]
    [InlineData(2, 10, 0)]
    [InlineData(2, 10, 10)]
    [InlineData(10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
    public void AddRoll_Singleplayer_FramesAreAddedCorrectly(int expectedNumberOfFrames, params int[] rolls)
    {
        var game = BowlingGame.StartNew(new[]{"Player"});
        foreach(var roll in rolls)
        {
            game.AddRoll(roll);
        }
        game.Frames[0].Length.Should().Be(expectedNumberOfFrames);
    }

    [Fact]
    public void AddRoll_SinglePlayer_GameIsLimitedToTenFrames()
    {
        var game = BowlingGame.StartNew(new[] {"Player"});

        // Add 10 frames
        for(int i = 0; i<20; i++)
        {
            game.AddRoll(0);
        }

        var exception = Record.Exception(() => game.AddRoll(0));
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 1, 0)]
    [InlineData(2, 0, 1)]
    [InlineData(2, 1, 0)]
    [InlineData(3, 0, 1)]
    [InlineData(3, 1, 1)]
    [InlineData(4, 0, 1)]
    [InlineData(4, 1, 1)]
    [InlineData(5, 0, 2)]
    [InlineData(5, 1, 1)]
    [InlineData(6, 0, 2)]
    [InlineData(6, 1, 1)]
    [InlineData(7, 0, 2)]
    [InlineData(7, 1, 2)]
    [InlineData(38, 0, 10)]
    [InlineData(38, 1, 9)]
    [InlineData(40, 1, 10)]
    public void AddRoll_TwoPlayers_FramesAreAddedCorrectly(int numberOfRolls, int playerIndex, int expectedNumberOfFrames)
    {
        var game = BowlingGame.StartNew(new[]{"Player1", "Player2"});
        for (int i = 0; i < numberOfRolls; i++)
        {
            game.AddRoll(0);
        }
        game.Frames[playerIndex].Length.Should().Be(expectedNumberOfFrames);
    }

    [Fact]
    public void AddRoll_TwoPlayers_GameIsLimitedToTenFrames()
    {
        var game = BowlingGame.StartNew(new[] {"Player1", "Player2"});

        // Add 10 frames
        for(int i = 0; i<40; i++)
        {
            game.AddRoll(0);
        }

        var exception = Record.Exception(() => game.AddRoll(0));
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void AddRoll_Singleplayer_TotalScoreIsCalculatedCorrectlyForStrike()
    {
        var game = BowlingGame.StartNew(new[] { "Player" });
        
        game.AddRoll(10);
        game.AddRoll(5);
        game.AddRoll(3);

        game.Frames[0][0].Score.Should().Be(18);
    }

    [Fact]
    public void AddRoll_Singleplayer_TotalScoreIsCalculatedCorrectlyForSpare()
    {
        var game = BowlingGame.StartNew(new[] { "Player" });

        game.AddRoll(5);
        game.AddRoll(5);
        game.AddRoll(1);
        game.AddRoll(9);

        game.Frames[0][0].Score.Should().Be(11);
    }

    [Fact]
    public void AddRoll_TwoPlayers_TotalScoreIsCalculatedCorrectlyForStrike()
    {
        var game = BowlingGame.StartNew(new[] { "Player 1", "Player 2"});

        // first player
        game.AddRoll(0);
        game.AddRoll(0);

        // second player rolls strike
        game.AddRoll(10);

        // first player
        game.AddRoll(0);
        game.AddRoll(0);

        // second player
        game.AddRoll(4);
        game.AddRoll(4);

        game.Frames[1][0].Score.Should().Be(18);
    }

    [Fact]
    public void AddScrole_TwoPlayers_TotalScoreIsCalculatedCorrectlyForSpare()
    {
        var game = BowlingGame.StartNew(new string[] { "Player 1", "Player 2"});
        
        // first player
        game.AddRoll(0);
        game.AddRoll(0);

        // second player rolls spare
        game.AddRoll(4);
        game.AddRoll(6);

        // first player
        game.AddRoll(0);
        game.AddRoll(0);

        // second player
        game.AddRoll(4);
        game.AddRoll(5);

        game.Frames[1][0].Score.Should().Be(14);
    }
}