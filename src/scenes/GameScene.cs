using System.Collections.Generic;
using System.Drawing;
using DotFeather;

public class GameScene : Scene
{
    public GameMode Mode { get; set; }

    public override void OnStart(Dictionary<string, object> args)
    {
        game = new Reversi();

        if (args.ContainsKey("mode") && args["mode"] is string mode)
        {
            switch (mode)
            {
                case "Multiplayer":
                    Mode = GameMode.MultiPlayer;
                    break;
                case "SimpleAI":
                    ai = new SimpleAi(game);
                    Mode = GameMode.VsCpu;
                    break;
                case "DynamicAI":
                    Mode = GameMode.VsCpu;
                    break;
                case "RandomAI":
                    ai = new RandomAi(game);
                    Mode = GameMode.VsCpu;
                    break;
                default:
                    break;

            }
        }

        boardView = new Container();
        boardView.Scale *= 2;
        Root.Add(boardView);

        var title = new TextElement("リバーシするヤーツ", DFFont.GetDefault(32), Color.White)
        {
            Location = (32, 32)
        };
        Root.Add(title);

        turn = new TextElement("のターン", DFFont.GetDefault(16), Color.White);
        Root.Add(turn);

        score = new TextElement("黒 / 白", DFFont.GetDefault(16), Color.White);
        Root.Add(score);

        tileStoneBlack = Tile.LoadFrom("resources/textures/stone-black.png");
        tileStoneWhite = Tile.LoadFrom("resources/textures/stone-white.png");
        tilePrompt = Tile.LoadFrom("resources/textures/prompt.png");

        var backdrop = new Sprite("resources/textures/board.png");
        boardView.Add(backdrop);

        stones = new Tilemap((16, 16));
        boardView.Add(stones);

        aud.Play(bgmMain, 0);
        boardView.Location = (DF.Window.Size.X / 2 - 128, 96);

        Render();
    }

    public override void OnUpdate()
    {
        boardView.Location = (DF.Window.Size.X / 2 - 128, 96);

        if (!DF.Window.IsFocused) return;

        if (DFKeyboard.Escape.IsKeyDown) DF.Router.ChangeScene<TitleScene>();

        if (game.IsGameSet)
        {
            if (DFMouse.IsLeftDown)
            {
                DF.Router.ChangeScene<TitleScene>();
            }
        }
        else
        {
            DoGame();
        }
    }

    public override void OnDestroy()
    {
        tileStoneBlack.Destroy();
        tileStoneWhite.Destroy();
        aud.Dispose();
    }

    public VectorInt? HoveringPosition
    {
        get
        {
            // 盤面の外にいたらnullを返す
            var (mx, my) = DFMouse.Position;
            var (bx, by) = boardView.Location;
            if (mx < bx || my < by || bx + 256 < mx || by + 256 < my) return null;
            return (VectorInt)(DFMouse.Position - boardView.Location) / 32;
        }
    }

    private void Render()
    {
        // Render Board
        stones.Clear();
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                var visiblePrompt = Mode switch
                {
                    GameMode.MultiPlayer => game.IsPlaceable((x, y)),
                    GameMode.VsCpu => game.CurrentStone == Stone.Black && game.IsPlaceable((x, y)),
                    GameMode.CpuVsCpu => false,
                    _ => false,
                };

                stones[x, y] = game.Board[x, y] switch
                {
                    Stone.Black => tileStoneBlack,
                    Stone.White => tileStoneWhite,
                    _ => visiblePrompt ? tilePrompt : null,
                };
            }
        }

        // Render UI
        Cls();

        var g = game.BlackCount == game.WhiteCount ? "引き分け!" : game.BlackCount > game.WhiteCount ? "黒の勝ち!" : "白の勝ち!";

        var t = (game.CurrentStone == Stone.Black ? "黒" : "白") + "のターン";

        turn.Text = game.IsGameSet ? g : t;
        score.Text = $"黒 {game.BlackCount:##} / 白 {game.WhiteCount:##}";

        score.Location = (DF.Window.Width / 2 - score.Width / 2, boardView.Location.Y + 256 + 16);
        turn.Location = (DF.Window.Width / 2 - turn.Width / 2, score.Location.Y + 16);
    }

    private void DoGame()
    {
        if (Mode == GameMode.VsCpu) DoVsCpu();
        if (Mode == GameMode.MultiPlayer) DoMultiplayer();
    }

    private void DoVsCpu()
    {
        if (game.CurrentStone == Stone.White)
        {
            if (ai != null && ai.HasResult && ai.Read() is VectorInt v)
            {
                game.Place(v);
                aud.PlayOneShotAsync(sfxPlace2);
                Render();
            }
            else if (ai != null && !ai.IsThinking)
            {
                ai.Think();
            }
        }
        else
        {
            if (DFMouse.IsLeftDown && HoveringPosition is VectorInt p)
            {
                if (!game.IsPlaceable(p))
                {
                    aud.PlayOneShotAsync(sfxError);
                }
                else
                {
                    game.Place(p);
                    aud.PlayOneShotAsync(sfxPlace1);
                }
                Render();
            }
        }
    }

    private void DoMultiplayer()
    {
        if (DFMouse.IsLeftDown && HoveringPosition is VectorInt p)
        {
            if (!game.IsPlaceable(p))
            {
                aud.PlayOneShotAsync(sfxError);
            }
            else
            {
                aud.PlayOneShotAsync(game.CurrentStone == Stone.Black ? sfxPlace1 : sfxPlace2);
                game.Place(p);
            }
            Render();
        }
    }

    private Container boardView;
    private Tilemap stones;
    private Tile tileStoneBlack;
    private Tile tileStoneWhite;
    private Tile tilePrompt;
    private TextElement turn;
    private TextElement score;

    private Reversi game;
    private IAi ai;

    private readonly AudioPlayer aud = new AudioPlayer();
    private readonly IAudioSource sfxPlace1 = new WaveAudioSource("resources/sounds/sfx_place1.wav");
    private readonly IAudioSource sfxPlace2 = new WaveAudioSource("resources/sounds/sfx_place2.wav");
    private readonly IAudioSource sfxError = new WaveAudioSource("resources/sounds/sfx_error.wav");
    private readonly IAudioSource bgmMain = new VorbisAudioSource("resources/sounds/bgm_main.ogg");
}
