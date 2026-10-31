using Godot;
using System;
using System.Collections.Generic;

namespace WAHA;

public class EntryCellFactory
{
    readonly Dictionary<string, EntryCell> cells;

    public EntryCellFactory(ScreenData screen)
    {
        var layout = screen.Layout
            ?? throw new InvalidOperationException("Entry cells require a screen layout block");
        var character = Assets.Instance.Tex(layout.Character);
        cells = new Dictionary<string, EntryCell>
        {
            ["icon"] = new IconCell(screen, character),
            ["color"] = new ColorCell(screen, character),
        };
    }

    public Button Build(ScreenData screen, string cellId, SelectionEntry entry)
        => cells.TryGetValue(cellId, out var cell) ? cell.Build(screen, entry)
           : throw new InvalidOperationException($"Unknown entry cell: {cellId} (entry {entry.Id})");
}
