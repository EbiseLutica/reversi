using DotFeather;

DF.Window.Mode = WindowMode.Resizable;

static void Run(DFKeyEventArgs _)
{
    if (DF.Window.IsFocused)
    {
        DF.Router.ChangeScene<ReversiScene>();
        DFKeyboard.KeyDown -= Run;
    }
}

DFKeyboard.KeyDown += Run;

return DF.Run();