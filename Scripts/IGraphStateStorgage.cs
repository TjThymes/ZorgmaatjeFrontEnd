

public interface IGraphStateStorage
{
    void SaveGraphState(string key, string value);
    string LoadGraphState(string key);
    void ClearGraphState();
}
