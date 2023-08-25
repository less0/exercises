using System.Diagnostics.CodeAnalysis;
using bowling_backend_core.DomainModel;
using bowling_backend_persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace bowling_backend_persistence_tests;

public class BowlingGameRepositoryTests : IAssemblyFixture<DockerDatabaseFixture>
{
    private readonly RawDbAccess _rawDbAccess = new(Constants.ServerConnectionString);
    private readonly BowlingGameRepository _repository;
    private IConfigurationRoot _configuration;
    private BowlingDataContext _dataContext;

    public BowlingGameRepositoryTests()
    {
        InitializeConfiguration();
        _rawDbAccess.WaitForConnection();
        InitializeDatabase();
        _rawDbAccess.ClearDatabase();

        _repository = new BowlingGameRepository(_dataContext);
    }

    [Fact]
    public void Save_GameIsSaved()
    {
        var userId = Guid.NewGuid().ToString();

        var game = BowlingGame.StartNew(new[]{ "Player" });
        _repository.Save(game, userId);
        _rawDbAccess.NumberOfRows().Should().Be(1);
    }

    [Fact]
    public void Save_UserIdIsSavedWithGame()
    {
        var userId = Guid.NewGuid().ToString();
        var game = BowlingGame.StartNew(new[] { "Player" });

        _repository.Save(game, userId);

        _rawDbAccess.GetValueByGameId<string>(game.Id, "UserId").Should().Be(userId);
    }

    [Fact]
    public void Save_PlayerNamesAreSavedWithGame()
    {
        var userId = Guid.NewGuid().ToString();
        var game = BowlingGame.StartNew(new[] { "Huey", "Dewie", "Louie" });

        _repository.Save(game, userId);

        _rawDbAccess.GetValueByGameId<string>(game.Id, "PlayerNames").Should().Be("Huey,Dewie,Louie");
    }

    [Theory]
    [InlineData(0, "10", 9, false)]
    [InlineData(1, "4,5", 0, false)]
    [InlineData(9, "0,0", 0, true)]
    public void Save_FrameDataIsSavedCorrectly(int frameIndex, string expectedRolls, int expectedBonusPoints, bool expectedIsLastFrame)
    {
        var userId = Guid.NewGuid().ToString();
        var game = BowlingGame.StartNew(new[] { "Player" });
        game.AddRoll(10);
        game.AddRoll(4);
        game.AddRoll(5);

        for (int i = 0; i < 16; i++)
        {
            game.AddRoll(0);
        }
    
        _repository.Save(game, userId);
    
        var frameId = game.Frames[0][frameIndex].Id;

        _rawDbAccess.GetValueByFrameId<string>(frameId, "Rolls").Should().Be(expectedRolls);
        _rawDbAccess.GetValueByFrameId<int>(frameId, "BonusPoints").Should().Be(expectedBonusPoints);
        _rawDbAccess.GetValueByFrameId<bool>(frameId, "IsLastFrame").Should().Be(expectedIsLastFrame);
    }

    [Fact]
    public void GetByEntityIdAndUserId_GameIsLoaded()
    {
        var game = BowlingGame.StartNew(new[] { "Player 1", "Player 2"});
        var userId = Guid.NewGuid().ToString();

        _repository.Save(game, userId);

        var loadedGame = _repository.GetByEntityIdAndUserId(game.Id, userId);

        loadedGame.Id.Should().Be(game.Id);
        loadedGame.PlayerNames.Should().BeEquivalentTo(game.PlayerNames, o => o.WithStrictOrdering());
    }

    [Fact]
    public void GetByEntityIdAndUserId_FramesAreLoadedInCorrectOrder()
    {
        var userId = Guid.NewGuid().ToString();
        var game = BowlingGame.StartNew(new[] { "Player 1", "Player 2", "Player 3" });
        for (int i = 0; i < 60; i++)
        {
            game.AddRoll(0);
        }

        _repository.Save(game, userId);

        var loadedGame = _repository.GetByEntityIdAndUserId(game.Id, userId);

        loadedGame.Frames.Should().NotBeEmpty();
        loadedGame.Frames.Should().HaveCount(3);

        for (int playerIndex = 0; playerIndex < 3; playerIndex++)
        {
            for (int frameIndex = 0; frameIndex < 10; frameIndex++)
            {
                var loadedFrame = loadedGame.Frames[playerIndex][frameIndex];
                var originalFrame = game.Frames[playerIndex][frameIndex];

                loadedFrame.Id.Should().Be(originalFrame.Id);
            }
        }
    }

    [Theory]
    [InlineData(0, "10", 9, false)]
    [InlineData(1, "4,5", 0, false)]
    [InlineData(2, "0,0", 0, false)]
    [InlineData(9, "0,0", 0, true)]
    public void Save_ChangesAreSaved(int frameIndex, string expectedRolls, int expectedBonusPoints, bool expectedIsLastFrame)
    {
        var userId = Guid.NewGuid().ToString();
        var originalGame = BowlingGame.StartNew(new[] { "A" });

        _repository.Save(originalGame, userId);

        var loadedGame = _repository.GetByEntityIdAndUserId(originalGame.Id, userId);

        loadedGame.AddRoll(10);
        loadedGame.AddRoll(4);
        loadedGame.AddRoll(5);
        for (int i = 0; i < 16; i++)
        {
            loadedGame.AddRoll(0);
        }

        _repository.Save(loadedGame, userId);

        _rawDbAccess.GetValueByFrameId<string>(loadedGame.Frames[0][frameIndex].Id, "Rolls").Should().Be(expectedRolls);
        _rawDbAccess.GetValueByFrameId<int>(loadedGame.Frames[0][frameIndex].Id, "BonusPoints").Should().Be(expectedBonusPoints);
        _rawDbAccess.GetValueByFrameId<bool>(loadedGame.Frames[0][frameIndex].Id, "IsLastFrame").Should().Be(expectedIsLastFrame);
    }

    [MemberNotNull(nameof(_dataContext))]
    private void InitializeDatabase()
    {
        _dataContext = new BowlingDataContext(_configuration);
        _dataContext.Database.Migrate();
    }

    [MemberNotNull(nameof(_configuration))]
    private void InitializeConfiguration()
    {
        ConfigurationBuilder configurationBuilder = new();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:(Default)"] = Constants.DatabaseConnectionString
        });
        _configuration = configurationBuilder.Build();
    }
}