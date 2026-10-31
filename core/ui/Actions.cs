using Godot;
using System;

namespace WAHA;

public static class Actions
{
    // screens subscribe to own their verbs (e.g. tab:*); unsubscribes on TreeExited
    public static event Action<Control, string>? ScreenAction;

    public static void Run(Control source, string action)
    {
        if (action.Length == 0) return; // visual-only button, no action wired in data
        if (action.StartsWith("tab:"))
        {
            if (ScreenAction == null)
                throw new InvalidOperationException($"Action {action} has no screen handler — screen failed to wire Actions.ScreenAction");
            ScreenAction(source, action);
            return;
        }
        if (action.StartsWith("scene:"))
        {
            source.GetTree().ChangeSceneToFile(action["scene:".Length..]);
            return;
        }
        if (action == "exit")
        {
            source.GetTree().Quit();
            return;
        }
        throw new InvalidOperationException($"Unknown action: {action}");
    }
}
