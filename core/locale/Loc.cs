using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WAHA;

public partial class Loc : Node
{
    public static Loc Instance { get; private set; } = null!;

    readonly Dictionary<string, string> _en = new();
    readonly Dictionary<string, string> _active = new();

    public override void _Ready()
    {
        Instance = this;

        var settingsPath = DataFiles.JsonFiles("res://data/locale")
            .FirstOrDefault(path => path.GetFile() == "settings.json");
        if (settingsPath == null)
            throw new InvalidOperationException("Missing locale settings: data/locale/settings.json");
        var lang = DataFiles.Str(DataFiles.LoadFile(settingsPath), "default");

        foreach (var (key, value) in LoadLanguageFolder("res://data/locale/en", "master locale en"))
            _en[key] = value;
        if (lang != "en")
            foreach (var (key, value) in LoadLanguageFolder($"res://data/locale/{lang}", lang))
            {
                if (!_en.ContainsKey(key))
                    throw new KeyNotFoundException(
                        $"Translation {lang} defines unknown key: {key} (not in en master; translators never invent keys)");
                _active[key] = value;
            }

        GD.Print($"Locale: '{lang}' active over en master ({_active.Count}/{_en.Count} keys translated)");
    }

    public string Tr(string key)
        => _active.TryGetValue(key, out var active) ? active
           : _en.TryGetValue(key, out var fallback) ? fallback
           : throw new KeyNotFoundException($"Missing locale key: {key}");

    static Dictionary<string, string> LoadLanguageFolder(string dir, string name)
    {
        var paths = DataFiles.FilesRecursive(dir).Where(path => path.GetExtension() == "json").ToList();
        if (paths.Count == 0)
            throw new InvalidOperationException($"Missing {name}: {dir} holds no json files");
        var flat = new Dictionary<string, string>();
        foreach (var path in paths)
            Flatten(DataFiles.LoadFile(path), "", flat);
        return flat;
    }

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
