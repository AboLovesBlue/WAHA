using Godot;
using System;
using System.Collections.Generic;

namespace WAHA;

public partial class Assets : Node
{
    public static Assets Instance { get; private set; } = null!;

    readonly Dictionary<string, string> _byRel = new();

    public override void _Ready()
    {
        Instance = this;
        AddLayer("res://assets");
        foreach (var mod in ModFolders()) AddLayer(mod);
    }

    public Texture2D Tex(string id)
        => _byRel.TryGetValue($"{id}.png", out var path) ? GD.Load<Texture2D>(path)
           : throw new InvalidOperationException($"Missing texture: {id} (searched all asset layers)");

    void AddLayer(string dir)
    {
        foreach (var path in DataFiles.FilesRecursive(dir))
            _byRel[path.Substring(dir.Length + 1)] = path;
    }

    List<string> ModFolders()
    {
        var list = new List<string>();
        var access = DirAccess.Open("res://mods");
        if (access == null) return list;

        access.ListDirBegin();
        var name = access.GetNext();
        while (name.Length > 0)
        {
            if (access.CurrentIsDir() && !name.StartsWith("."))
                list.Add($"res://mods/{name}");
            name = access.GetNext();
        }
        access.ListDirEnd();
        list.Sort();
        return list;
    }
}
