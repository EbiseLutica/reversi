using System.Collections.Generic;
using System.Drawing;
using DotFeather;

public class TitleScene : Scene
{
    public override void OnStart(Dictionary<string, object> args)
    {
        Cls();
        Print("リバーシするヤーツ");
        Print("");
        Print("[1] vs CPU (シンプル)");
        Print("[2] マルチプレイ");
        Print("[3] vs CPU (ランダム)");
        Print("[4] vs CPU (ダイナミック) 未実装");
        Print("[5] CPU (ランダム) vs CPU (ランダム)");
    }

    public override void OnUpdate()
    {
        var mode = DFKeyboard.Number1.IsKeyDown  ? "SimpleAI" : 
        DFKeyboard.Number2.IsKeyDown ? "Multiplayer" :
        DFKeyboard.Number3.IsKeyDown ? "RandomAI" :
        DFKeyboard.Number4.IsKeyDown ? "DynamicAI" :
        DFKeyboard.Number5.IsKeyDown ? "SimpleAI vs SimpleAI" :
        null;

        if (mode is not null)
        {
            DF.Router.ChangeScene<GameScene>(new Dictionary<string, object>{ { "mode", mode } });
        }
    }

    public override void OnDestroy()
    {
        
    }
}
