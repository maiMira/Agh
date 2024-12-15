namespace Agh.Domain.Base.Notifications;

public class Notification
{
    private readonly List<string> _messages = new List<string>();
    public IReadOnlyList<string> Messages => _messages;

    public void AddMessage(string message)
    {
        _messages.Add(message);
    }

    public bool HasMessages => _messages.Any();
}