using Godot;
using System;

namespace WAHA;

// ui ids are the stable address of every ui element (ui.<screen>.<widget>) so tools
// can address elements by id; the id travels as node metadata, the node name is the
// short tail of the id for readability
public static class UiId
{
    public static void Tag(Node node, ScreenData screen, string slot)
    {
        if (!screen.Ids.TryGetValue(slot, out var id))
            throw new InvalidOperationException(
                $"Missing ui id slot: {slot} (screen {screen.ScreenId}, ids: {string.Join(", ", screen.Ids.Keys)})");
        node.SetMeta("ui_id", id);
        node.Name = id[(id.LastIndexOf('.') + 1)..];
    }
}
