using bowling_backend_persistence.DataModel;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using BowlingGameDomain = bowling_backend_core.DomainModel.BowlingGame;

namespace bowling_backend_persistence_tests.DataModel;

public class BowlingGameMapperTests
{
    [Fact]
    public void Map_NewGameIsMappedCorrectly()
    {
        BowlingGameMapper mapper = new();

        var game = BowlingGameDomain.StartNew(new[] { "Player" });

        var mappedGame = mapper.Map(game);

        mappedGame.Id.Should().Be(game.Id);
        mappedGame.PlayerNames.Should().BeEquivalentTo(new[] { "Player" });
    }

    [Theory]
    [InlineData("Paul")]
    [InlineData("Huey", "Dewey", "Louie")]
    public void Map_PlayerNamesAreMappedCorrectly(params string[] playerNames)
    {
        BowlingGameMapper mapper = new();

        var game = BowlingGameDomain.StartNew(playerNames);

        var mappedGame = mapper.Map(game);

        mappedGame.PlayerNames.Should().BeEquivalentTo(playerNames);
    }

    [Fact]
    public void Map_NewGame_UserIdIsEmpty()
    {
        BowlingGameMapper mapper = new();
        var game = BowlingGameDomain.StartNew(new[] { "Player" });

        var mappedGame = mapper.Map(game);

        mappedGame.UserId.Should().BeEmpty();
    }

    [Theory]
    [InlineData(3, 0, 0)]
    [InlineData(3, 1, 1)]
    [InlineData(40, 9, 0)]
    [InlineData(40, 10, 1)]
    [InlineData(40, 19, 1)]
    public void Map_TwoPlayers_FramesAreAddedWithCorrectPlayerIndices(int numberOfRolls, int frameIndex, int expectedPlayerIndex)
    {
        BowlingGameMapper mapper = new();
        var game = BowlingGameDomain.StartNew(new[] { "Player 1", "Player 2" });

        for (int i = 0; i < numberOfRolls; i++)
        {
            game.AddRoll(0);
        }

        var mappedGame = mapper.Map(game);

        mappedGame.Frames[frameIndex].PlayerIndex.Should().Be(expectedPlayerIndex);
    }

    [Theory]
    [InlineData(7, 0, 0)]
    [InlineData(7, 1, 1)]
    [InlineData(7, 2, 2)]
    [InlineData(7, 3, 3)]
    [InlineData(15, 0, 0)]
    [InlineData(15, 1, 0)]
    [InlineData(15, 6, 3)]
    [InlineData(15, 7, 3)]
    [InlineData(80, 9, 0)]
    [InlineData(80, 10, 1)]
    [InlineData(80, 19, 1)]
    [InlineData(80, 20, 2)]
    [InlineData(80, 29, 2)]
    [InlineData(80, 30, 3)]
    public void Map_FourPlayers_FramesAreAddedWithCorrectPlayerIndices(int numberOfRolls, int frameIndex, int expectedPlayerIndex)
    {
        BowlingGameMapper mapper = new();
        var game = BowlingGameDomain.StartNew(new[] { "Player 1", "Player 2", "Player 3", "Player 4" });

        for (int i = 0; i < numberOfRolls; i++)
        {
            game.AddRoll(0);
        }

        var mappedGame = mapper.Map(game);

        mappedGame.Frames[frameIndex].PlayerIndex.Should().Be(expectedPlayerIndex);
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(0, 9, 9)]
    [InlineData(1, 0, 10)]
    [InlineData(1, 1, 11)]
    [InlineData(1, 9, 19)]
    [InlineData(2, 0, 20)]
    [InlineData(2, 4, 24)]
    [InlineData(2, 9, 29)]
    [InlineData(3, 0, 30)]
    [InlineData(3, 3, 33)]
    [InlineData(3, 9, 39)]
    public void Map_CorrectFramesAreMapped(int playerIndex, int frameIndex, int mappedFrameIndex)
    {
        BowlingGameMapper mapper = new();
        var game = BowlingGameDomain.StartNew(new[] { "Player 1", "Player 2", "Player 3", "Player 4"});

        for (int i = 0; i < 80; i++)
        {
            game.AddRoll(0);
        }
        
        var mappedGame = mapper.Map(game);

        mappedGame.Frames[mappedFrameIndex].Id.Should().Be(game.Frames[playerIndex][frameIndex].Id);
    }

    [Fact]
    public void Map_BonusPointsIsMappedCorrectly()
    {
        BowlingGameMapper mapper = new();
        var game = BowlingGameDomain.StartNew(new [] { "Player 1" });

        // first frame
        game.AddRoll(10);

        // second frame
        game.AddRoll(4);
        game.AddRoll(3);

        var mappedGame = mapper.Map(game);

        mappedGame.Frames[0].BonusPoints.Should().Be(7);
    }

    [Fact]
    public void Map_RollsAreMappedCorrectly()
    {
        BowlingGameMapper mapper = new();
        var game = BowlingGameDomain.StartNew(new[] { "Player" });

        // first frame
        game.AddRoll(3);
        game.AddRoll(7);

        // second frame
        game.AddRoll(5);
        game.AddRoll(4);

        var mappedGame = mapper.Map(game);

        mappedGame.Frames[0].Rolls.Should().BeEquivalentTo(new[] { 3, 7 });
        mappedGame.Frames[1].Rolls.Should().BeEquivalentTo(new[] { 5, 4 });
    }

    [Fact]
    public void Map_IsLastFrameIsMappedCorrectly()
    {
        BowlingGameMapper mapper = new();
        var game = BowlingGameDomain.StartNew(new[] { "Player" });
        for (int i = 0; i < 20; i++)
        {
            game.AddRoll(0);
        }

        var mappedGame = mapper.Map(game);
    
        mappedGame.Frames[0].IsLastFrame.Should().BeFalse();
        mappedGame.Frames[8].IsLastFrame.Should().BeFalse();
        mappedGame.Frames[9].IsLastFrame.Should().BeTrue();
    }
}