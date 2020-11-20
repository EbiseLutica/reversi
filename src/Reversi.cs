using System;
using System.Collections.Generic;
using System.Linq;
using DotFeather;

public class Reversi
{
    public Stone CurrentStone { get; private set; }

    public VectorInt[] PlaceableLocations { get; private set; }

    public bool IsGameSet { get; private set; }

    public Stone[,] Board { get; }

    public int BlackCount { get; private set; }

    public int WhiteCount { get; private set; }

    public Reversi(Stone beginStone = Stone.Black)
    {
        Board = new Stone[8, 8];
        CurrentStone = beginStone;

        // init
        Board[3, 3] = Stone.Black;
        Board[3, 4] = Stone.White;
        Board[4, 3] = Stone.White;
        Board[4, 4] = Stone.Black;

        Update();
    }

    public void Place(VectorInt location)
    {
        var turnables = GetTurnableStones(location);
        if (turnables.Count == 0) throw new ArgumentException(null, nameof(location));

        Board[location.X, location.Y] = CurrentStone;
        turnables.ForEach(l => Board[l.X, l.Y] = CurrentStone);

        CurrentStone = CurrentStone.Invert();
        Update();
        if (PlaceableLocations.Length == 0)
        {
            // パス
            CurrentStone = CurrentStone.Invert();
            Update();
            if (PlaceableLocations.Length == 0)
            {
                // ゲームセット
                IsGameSet = true;
            }
        }
    }

    public bool IsPlaceable(VectorInt location)
    {
        return PlaceableLocations.Contains(location);
    }

    public List<VectorInt> GetTurnableStones(VectorInt location)
    {
        var turnables = new List<VectorInt>();

        //範囲外だとおけない
        var isOutOfRange = location.X < 0 || location.Y < 0 || 7 < location.X || 7 < location.Y;

        // 無をおけない
        var isStoneIsNone = CurrentStone == Stone.None;

        // その位置に石があればおけない
        var isReserved = Board[location.X, location.Y] != Stone.None;

        if (isOutOfRange || isStoneIsNone || isReserved) return turnables;

        turnables.AddRange(GetTurnableStones(location, VectorInt.Up));
        turnables.AddRange(GetTurnableStones(location, VectorInt.Down));
        turnables.AddRange(GetTurnableStones(location, VectorInt.Left));
        turnables.AddRange(GetTurnableStones(location, VectorInt.Right));
        turnables.AddRange(GetTurnableStones(location, VectorInt.Up + VectorInt.Left));
        turnables.AddRange(GetTurnableStones(location, VectorInt.Up + VectorInt.Right));
        turnables.AddRange(GetTurnableStones(location, VectorInt.Down + VectorInt.Left));
        turnables.AddRange(GetTurnableStones(location, VectorInt.Down + VectorInt.Right));

        return turnables;
    }

    private IEnumerable<VectorInt> GetTurnableStones(VectorInt location, VectorInt way)
    {
        var t = new List<VectorInt>();
        var cur = location;

        cur += way;

        var yourStone = CurrentStone.Invert();
        var endsWithMyStone = false;

        while (0 <= cur.X && 0 <= cur.Y && cur.X <= 7 && cur.Y <= 7)
        {
            var s = Board[cur.X, cur.Y];
            endsWithMyStone = s == CurrentStone;
            if (s == yourStone) t.Add(cur); else break;

            cur += way;
        }

        return endsWithMyStone ? t : Array.Empty<VectorInt>();
    }

    private void Update()
    {
        var l = new List<VectorInt>();
        int b = 0, w = 0;
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                if (GetTurnableStones((x, y)).Count > 0) l.Add((x, y));
                if (Board[x, y] == Stone.Black) b++;
                if (Board[x, y] == Stone.White) w++;
            }
        }

        BlackCount = b;
        WhiteCount = w;
        PlaceableLocations = l.ToArray();
    }
}