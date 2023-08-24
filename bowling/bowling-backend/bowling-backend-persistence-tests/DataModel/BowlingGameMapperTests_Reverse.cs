using bowling_backend_persistence.DataModel;
using FluentAssertions;

namespace bowling_backend_persistence_tests.DataModel;

public partial class BowlingGameMapperTests
{
    [Fact]
    public void MapReverse_IdIsMappedCorrectly()
    {
        BowlingGameMapper mapper = new();
        BowlingGame game = new()
        {
            Id = Guid.NewGuid(),
            PlayerNames = new[] { "A" },
            UserId = string.Empty,
            Frames = new()
        };

        var mappedGame = mapper.ReverseMap(game);

        mappedGame.Id.Should().Be(game.Id);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Player 1", "Player 2")]
    [InlineData("Tick", "Trick", "Track")]
    public void MapReverse_PlayerNamesAreMappedCorrectly(params string[] playerNames)
    {
        BowlingGameMapper mapper = new();
        BowlingGame game = new()
        {
            Id = Guid.NewGuid(),
            PlayerNames = playerNames,
            UserId = string.Empty,
            Frames = new()
        };

        var mappedGame = mapper.ReverseMap(game);

        mappedGame.PlayerNames.Should().BeEquivalentTo(playerNames, o => o.WithStrictOrdering());
    }

    [Fact]
    public void MapReverse_SinglePlayer_FramesAreMappedCorrectly()
    {
        BowlingGameMapper mapper = new();

        List<Frame> frames = new()
        {
            new Frame()
            {
                Id = Guid.NewGuid(),
                PlayerIndex = 0,
                Rolls = new[] { 3, 3 }
            },
            new Frame()
            {
                Id = Guid.NewGuid(),
                PlayerIndex = 0,
                Rolls = new[] { 10 },
                BonusPoints = 6
            },
            new Frame()
            {
                Id = Guid.NewGuid(),
                PlayerIndex = 0,
                Rolls = new[] { 2, 4 }
            }
        };

        BowlingGame game = new()
        {
            Id = Guid.NewGuid(),
            PlayerNames = new[] { "Player" },
            UserId = string.Empty,
            Frames = frames
        };

        var mappedGame = mapper.ReverseMap(game);

        mappedGame.Frames.Should().HaveCount(1);
        mappedGame.Frames[0].Should().HaveCount(3);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(9, 0, 9)]
    [InlineData(10, 1, 0)]
    [InlineData(12, 1, 2)]
    [InlineData(19, 1, 9)]
    [InlineData(20, 2, 0)]
    [InlineData(25, 2, 5)]
    [InlineData(29, 2, 9)]
    public void MapReverse_FramesAreMappedCorrectly(int frameIndex, int mappedPlayerIndex, int mappedFrameIndex)
    {
        BowlingGameMapper mapper = new();

        BowlingGame game = new()
        {
            Id = Guid.NewGuid(),
            PlayerNames = new[] { "Player 1", "Player 2", "Player 3" },
            UserId = string.Empty,
            Frames = GenerateFramesForFullGame(3)
        };

        var mappedGame = mapper.ReverseMap(game);

        mappedGame.Frames[mappedPlayerIndex][mappedFrameIndex].Id.Should().Be(game.Frames[frameIndex].Id);
    }

    [Fact]
    public void MapReverse_FrameDataIsMappedCorrectly()
    {
        BowlingGameMapper mapper = new();

        List<Frame> frames = new()
        {
            new Frame
            {
                Id = Guid.NewGuid(),
                PlayerIndex = 0,
                Rolls = new[] { 4, 5 }
            },
            new Frame
            {
                Id = Guid.NewGuid(),
                PlayerIndex = 0,
                Rolls = new[] { 10 },
                BonusPoints = 6
            },
            new Frame
            {
                Id = Guid.NewGuid(),
                PlayerIndex = 0,
                Rolls = new[] { 2, 4 }
            }
        };

        BowlingGame game = new()
        {
            Id = Guid.NewGuid(),
            PlayerNames = new[] { "Player" },
            Frames = frames,
            UserId = string.Empty
        };

        var mappedGame = mapper.ReverseMap(game);

        mappedGame.Frames[0][0].Rolls.Should().BeEquivalentTo(new[] { 4, 5 }, o => o.WithStrictOrdering());
        mappedGame.Frames[0][0].Score.Should().Be(9);
        mappedGame.Frames[0][1].Rolls.Should().BeEquivalentTo(new[] { 10 });
        mappedGame.Frames[0][1].Score.Should().Be(16);
        mappedGame.Frames[0][2].Rolls.Should().BeEquivalentTo(new[] { 2, 4 }, o => o.WithStrictOrdering());
        mappedGame.Frames[0][2].Score.Should().Be(6);
    }

    private List<Frame> GenerateFramesForFullGame(int numberOfPlayers)
    {
        var result = new List<Frame>();

        for (int i = 0; i < numberOfPlayers * 10; i++)
        {
            var isLastFrame = ((i + 1) % 10 == 0);
            var playerIndex = i / 10;

            var frame = new Frame
            {
                Id = Guid.NewGuid(),
                Rolls = new[] { 0, 0 },
                BonusPoints = 0,
                IsLastFrame = isLastFrame,
                PlayerIndex = playerIndex
            };

            result.Add(frame);
        }

        return result;
    }
}