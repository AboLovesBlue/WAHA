using Godot;
using System;
using System.Collections.Generic;

namespace WAHA;

public class SelectionState
{
    readonly Dictionary<string, string> picked = new();
    readonly Dictionary<string, string> custom = new();

    public void Select(string source, string entryId) => picked[source] = entryId;

    public string Pick(string source) => picked.TryGetValue(source, out var id) ? id : "";

    public void SetCustom(string source, string color) => custom[source] = color;

    public string Custom(string source) => custom.TryGetValue(source, out var color) ? color : "";
}
