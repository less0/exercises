using bowling_backend_persistence.DataModel;
using FluentAssertions;

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
        var game = BowlingGameDomain.StartNew(new[] {"Player"});

        var mappedGame = mapper.Map(game);

        mappedGame.UserId.Should().BeEmpty();
    }

    [Fact]
    public void Map_FramesAreAddedWithCorrectIndices()
    {
        BowlingGameMapper mapper = new();
        var game = BowlingGameDomain.StartNew(new[] {"Player 1", "Player 2"});
        game.AddRoll(0);
        game.AddRoll(0);
        game.AddRoll(0);
        
        var mappedGame = mapper.Map(game);

        mappedGame.Frames.Should().HaveCount(2);
        mappedGame.Frames[0].PlayerIndex.Should().Be(0);
        mappedGame.Frames[1].PlayerIndex.Should().Be(1);
    }
}