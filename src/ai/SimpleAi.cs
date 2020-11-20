using System;
using System.Collections;
using System.Linq;
using DotFeather;

public class SimpleAi : IAi
{
    public SimpleAi(Reversi game)
    {
        this.game = game;
    }

    public bool HasResult => result is not null;

    public bool IsThinking { get; private set; }

    public void Think()
    {
        IsThinking = true;
        CoroutineRunner.Start(ThinkTask()).Then(_ => IsThinking = false);
    }

    public VectorInt? Read()
    {
        var t = result;
        result = null;
        return t;
    }

    private IEnumerator ThinkTask()
    {
        VectorInt? res = null;
        int minScore = int.MaxValue;
        var locs = game.PlaceableLocations;
        Console.WriteLine(string.Join('\n', locs.Select(l => "- " + l)));
        foreach (var l in locs)
        {
            Console.WriteLine("hmm I think about " + l);
            var score = game.GetTurnableStones(l).Count;
            if (minScore > score)
            {
                Console.WriteLine($"{l}:{score} is better than {res}:{minScore}");
                minScore = score;
                res = l;
            }
            yield return null;
        }
        Console.WriteLine("Finally I chose " + res);

        result = res;
    }

    private readonly Reversi game;
    private VectorInt? result;
}