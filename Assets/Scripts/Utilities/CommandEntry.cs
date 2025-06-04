using UnityEngine;

public class CommandEntry
{
    public ICommand Command { get; private set; }
    public bool RemoveOnExit { get; private set; }

    public bool HasEntered { get; set; } = false;

    public CommandEntry(ICommand command, bool removeOnExit = true)
    {
        Command = command;
        RemoveOnExit = removeOnExit;
    }
}
