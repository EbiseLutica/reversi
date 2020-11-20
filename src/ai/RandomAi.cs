using System;
using System.Collections;
using System.Linq;
using DotFeather;

public class RandomAi : IAi
{
    public RandomAi(Reversi game)
    {
        this.game = game;
    }

    public bool HasResult => result is not null;

    public bool IsThinking { get; private set; }

    public void Think()
    {
        IsThinking = true;

        if (game.PlaceableLocations.Length > 0)
        {
            result = game.PlaceableLocations[rnd.Next(game.PlaceableLocations.Length)];
            IsThinking = false;
        }
    }

    public VectorInt? Read()
    {
        var t = result;
        result = null;
        return t;
    }

    private readonly Reversi game;
    private VectorInt? result;
    private static Random? rnd = new Random();
}