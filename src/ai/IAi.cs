using DotFeather;

public interface IAi
{
    bool HasResult { get; }
    bool IsThinking { get; }
    void Think();
    VectorInt? Read();
}
