﻿namespace bowling_backend_core.DomainModel;

public class BowlingGame
{
    List<Frame>[] _frames;

    public static BowlingGame StartNew(string[] players)
    {
        if(players.Length == 0)
        {
            throw new ArgumentException("Cannot start a game without players.", nameof(players));
        }

        BowlingGame bowlingGame = new()
        {
            PlayerNames = players
        };
        bowlingGame.Init();

        return bowlingGame;
    }

    private BowlingGame()
    {}

    private void Init()
    {
        _frames = new List<Frame>[PlayerNames.Length];
        for(int i = 0; i<_frames.Length; i++)
        {
            _frames[i] = new();
        }
    }


    public string[] PlayerNames { get; private set; }
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Frame[][] Frames {
        get{
            return _frames.Select(l => l.ToArray()).ToArray();
        }
    }
    private bool IsFinished 
    {
        get
        {
            var lastFramesList = _frames.Last();
            return lastFramesList.Count == 10 && lastFramesList[9].IsFinished;
        }
    }

    public void AddRoll(int numberOfPins)
    {
        if(IsFinished)
        {
            throw new InvalidOperationException();
        }

        var playerIndex = GetCurrentPlayerIndex();
        var frames = _frames[playerIndex];

        if(!frames.Any())
        {
            frames.Add(Frame.FirstFrame(numberOfPins));
        }
        else if(frames.Last().IsFinished)
        {
            frames.Add(Frame.MidgameFrame(numberOfPins, 0));
        }
        else
        {
            frames.Last().AddRoll(numberOfPins);
        }
    }

    private int GetCurrentPlayerIndex() 
    {
        if(PlayerNames.Length == 1)
        {
            return 0;
        }

        if(TryGetActiveFramePlayerIndex(out int playerIndex))
        {
            return playerIndex;
        }

        return GetNextPlayerIndex();
    }

    private bool TryGetActiveFramePlayerIndex(out int playerIndex)
    {
        var activeFramePlayerIndex = _frames
            .Select((frames, index) => new{index, isActive=frames.Any(frame => !frame.IsFinished)})
            .Where(x => x.isActive)
            .Select(x => x.index)
            .DefaultIfEmpty(-1)
            .FirstOrDefault();
        
        playerIndex = activeFramePlayerIndex;
        return playerIndex >= 0;        
    }

    private int GetNextPlayerIndex()
    {
        if(AllFrameListsHaveSameLength())
        {
            return 0;
        }

        return GetPlayerIndexWithMoreFrames() + 1;
    }

    private bool AllFrameListsHaveSameLength()
    {
        return _frames.Select(frames => frames.Count)
                      .Distinct()
                      .Count() == 1;
    }

    private int GetPlayerIndexWithMoreFrames()
    {
        return _frames.Select((frames, index) => new{index, frames})
                      .MaxBy(x => x.frames.Count)
                      .index;
    }
}