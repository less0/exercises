using System.Diagnostics.CodeAnalysis;
using bowling_backend_core.DomainModel;
using FluentAssertions;

namespace bowling_backend_tests.DomainModel;

public partial class BowlingGameTests
{
    private const int Strike = 10;
    private static readonly string[] OnePlayer = new[]{ "Player" };
    private static readonly string[] TwoPlayers = new[]{ "Player 1", "Player 2" };
    private static readonly string[] ThreePlayers = new[]{ "Player 1", "Player 2", "Player 3" };
    private static readonly string[] FourPlayers = new[]{ "Player 1", "Player 2", "Player 3", "Player 4" };

    public static IEnumerable<object[]> GetPlayersCombinations()
    {
        yield return OnePlayer.Cast<object>().ToArray();
        yield return TwoPlayers.Cast<object>().ToArray();
        yield return ThreePlayers.Cast<object>().ToArray();
        yield return FourPlayers.Cast<object>().ToArray();
    }

    [Theory]
    [InlineData("Peter", "Paul", "Mary")]
    public void StartNew_PlayerNamesAreSet(params string[] playerNames)
    {
        var game = BowlingGame.StartNew(playerNames);
        game.PlayerNames.Should().BeEquivalentTo(playerNames, o => o.WithStrictOrdering());
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
        var game = BowlingGame.StartNew(OnePlayer);
        game.Id.Should().NotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(GetPlayersCombinations))]
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
        var game = BowlingGame.StartNew(OnePlayer);
        foreach(var roll in rolls)
        {
            game.AddRoll(roll);
        }
        game.Frames[0].Length.Should().Be(expectedNumberOfFrames);
    }

    [Theory]
    [MemberData(nameof(GetPlayersCombinations))]
    public void AddRoll_LastFrameIsTreatedSpecially(params string[] playerNames)
    {
        var game = BowlingGame.StartNew(playerNames);

        RollAllFramesButLast(game);

        game.AddRoll(Strike);
        var exception = Record.Exception(() => game.AddRoll(Strike));
        exception.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(GetPlayersCombinations))]  
    public void AddRoll_GameIsLimitedToTenFrames(params string[] players)
    {
        var game = BowlingGame.StartNew(players);

        RollAllFrames(game);

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
        var game = BowlingGame.StartNew(TwoPlayers);
        for (int i = 0; i < numberOfRolls; i++)
        {
            game.AddRoll(0);
        }
        game.Frames[playerIndex].Length.Should().Be(expectedNumberOfFrames);
    }

    [Fact]
    public void AddRoll_TwoPlayers_GameIsLimitedToTenFrames()
    {
        var game = BowlingGame.StartNew(TwoPlayers);

        RollAllFrames(game);

        var exception = Record.Exception(() => game.AddRoll(0));
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void AddRoll_Singleplayer_TotalScoreIsCalculatedCorrectlyForStrike()
    {
        var game = BowlingGame.StartNew(OnePlayer);
        
        game.AddRoll(Strike);
        game.AddRoll(5);
        game.AddRoll(3);

        game.Frames[0][0].Score.Should().Be(18);
    }

    [Fact]
    public void AddRoll_Singleplayer_TotalScoreIsCalculatedCorrectlyForSpare()
    {
        var game = BowlingGame.StartNew(OnePlayer);

        RollRandomSpare(game);
        game.AddRoll(1);
        game.AddRoll(9);

        game.Frames[0][0].Score.Should().Be(11);
    }

    [Fact]
    public void AddRoll_TwoPlayers_TotalScoreIsCalculatedCorrectlyForStrike()
    {
        var game = BowlingGame.StartNew(TwoPlayers);

        // first player
        game.AddRoll(0);
        game.AddRoll(0);

        // second player rolls strike
        game.AddRoll(Strike);

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
        var game = BowlingGame.StartNew(TwoPlayers);
        
        // first player
        game.AddRoll(0);
        game.AddRoll(0);

        // second player rolls spare
        RollRandomSpare(game);

        // first player
        game.AddRoll(0);
        game.AddRoll(0);

        // second player
        game.AddRoll(4);
        game.AddRoll(5);

        game.Frames[1][0].Score.Should().Be(14);
    }

    [Theory]
    [MemberData(nameof(GetPlayersCombinations))]
    public void IsFinished_ReturnsTrueAfterLastFrameIsFinished(params string[] players)
    {
        var game = BowlingGame.StartNew(players);
        RollAllFrames(game);
        game.IsFinished.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetPlayersCombinations))]
    public void IsFinished_IsNotFinishedAfterTwoStrikesInLastFrame(params string[] players)
    {
        var game = BowlingGame.StartNew(players);

        RollAllFramesButLast(game);
        game.AddRoll(Strike);
        game.AddRoll(Strike);

        // two strikes on the last frame won't finish the game
        game.IsFinished.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetPlayersCombinations))]
    public void IsFinished_IsNotFinishedAfterSpareInLastFrame(params string[] players)
    {
        var game = BowlingGame.StartNew(players);

        RollAllFramesButLast(game);
        RollRandomSpare(game);

        // spare on the last frame won't finish the game
        game.IsFinished.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetPlayersCombinations))]
    public void IsFinished_IsFinishedAfterBonusRollInLastFrame(params string[] players)
    {
        var game = BowlingGame.StartNew(players);

        RollAllFramesButLast(game);

        game.AddRoll(Strike);
        game.AddRoll(Strike);
        game.AddRoll(3);

        game.IsFinished.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetPlayersCombinations))]
    public void WinnerName_LastPlayer(params string[] players)
    {
        var game = BowlingGame.StartNew(players);

        RollAllFramesButLast(game);

        game.AddRoll(Strike);
        game.AddRoll(Strike);
        game.AddRoll(3);

        game.WinnerNames.Should().BeEquivalentTo(new[] { players.Last() });
    }

    [Fact]
    public void WinnerNames_GameIsNotFinished_IsEmpty()
    {
        var game = BowlingGame.StartNew(ThreePlayers);
        RollAllFramesButLast(game);
        game.WinnerNames.Should().BeEmpty();
    }

    [Fact]
    public void WinnerNames_SecondToLast()
    {
        var game = BowlingGame.StartNew(ThreePlayers);
        AddNRolls(game, 20 + 18 + 18);
        RollRandomFrame(game);
        RollNullFrame(game);

        game.WinnerNames.Should().BeEquivalentTo(new[] { ThreePlayers[^2] });
    }

    [Fact]
    public void WinnerNames_First()
    {
        var game = BowlingGame.StartNew(ThreePlayers);
        AddNRolls(game, 3 * 18);
        RollRandomFrame(game);
        RollNullFrame(game);
        RollNullFrame(game);

        game.WinnerNames.Should().BeEquivalentTo(new[] { ThreePlayers[0] });
    }

    [Fact]
    public void WinnerNames_Multiple()
    {
        var game = BowlingGame.StartNew(ThreePlayers);
        AddNRolls(game, 3 * 18);
        RollFrame(game, sum: 9);
        RollNullFrame(game);
        RollFrame(game, sum: 9);

        game.WinnerNames.Should().BeEquivalentTo(new[] {ThreePlayers[0], ThreePlayers[2]});
    }

    private void RollNullFrame(BowlingGame game)
    {
        game.AddRoll(0);
        game.AddRoll(0);
    }

    private void RollFrame(BowlingGame game, int sum)
    {
        var firstRoll = Random.Shared.Next(sum + 1);
        var secondRoll = sum - firstRoll;

        game.AddRoll(firstRoll);
        game.AddRoll(secondRoll);
    }

    private void RollRandomFrame(BowlingGame game)
    {
        var firstRoll = Random.Shared.Next(10);
        var secondRoll = Random.Shared.Next(10 - firstRoll);

        game.AddRoll(firstRoll);
        game.AddRoll(secondRoll);
    }

    private static void RollAllFrames(BowlingGame game)
    {
        var numberOfRolls = game.PlayerNames.Length * 20;
        for (int i = 0; i < numberOfRolls; i++)
        {
            game.AddRoll(0);
        }
    }

    private static void RollAllFramesButLast(BowlingGame game)
    {
        var numberOfRolls = game.PlayerNames.Length * 20 - 2;
        for (int i = 0; i < numberOfRolls; i++)
        {
            game.AddRoll(0);
        }
    }

    private static void RollRandomSpare(BowlingGame game)
    {
        var firstRoll = Random.Shared.Next(10);
        var secondRoll = 10 - firstRoll;

        game.AddRoll(firstRoll);
        game.AddRoll(secondRoll);
    }

    private static void AddNRolls(BowlingGame game, int numberOfRolls)
    {
        for (int i = 0; i < numberOfRolls; i++)
        {
            game.AddRoll(0);
        }
    }
}