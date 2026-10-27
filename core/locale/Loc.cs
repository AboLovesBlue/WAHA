using Godot;
using System;
using System.Collections.Generic;

namespace WAHA;

public partial class Loc : Node
{
    public static Loc Instance { get; private set; } = null!;

    readonly Dictionary<string, string> _en = new();

    public override void _Ready()
    {
        Instance = this;

        var enPath = "";
        foreach (var path in DataFiles.JsonFiles("res://data/locale"))
            if (path.GetFile() == "en.json")
                enPath = path;

        if (enPath.Length == 0)
            throw new InvalidOperationException("Missing master locale: res://data/locale/en.json");

        Flatten(DataFiles.LoadFile(enPath), "", _en);
    }

    public string Tr(string key)
        => _en.TryGetValue(key, out var value) ? value
           : throw new KeyNotFoundException($"Missing locale key: {key}");

    static void Flatten(Godot.Collections.Dictionary dict, string prefix, Dictionary<string, string> output)
    {
        foreach (var pair in dict)
        {
            var path = prefix.Length == 0 ? pair.Key.AsString() : $"{prefix}.{pair.Key.AsString()}";
            if (pair.Value.VariantType == Variant.Type.Dictionary)
                Flatten(pair.Value.AsGodotDictionary(), path, output);
            else
                output[path] = pair.Value.AsString();
        }
    }
}
