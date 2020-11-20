using DotFeather;

DF.Window.Mode = WindowMode.Resizable;

DF.Window.Start += () => DF.Router.ChangeScene<TitleScene>();

return DF.Run();