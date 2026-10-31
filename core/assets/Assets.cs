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
        => _byRel.TryGetValue(id, out var path) ? Load(path)
           : throw new InvalidOperationException($"Missing texture: {id} (searched all asset layers)");

    public Font Font(string id)
        => _byRel.TryGetValue(id, out var path) ? GD.Load<Font>(path)
           : throw new InvalidOperationException($"Missing font: {id} (searched all asset layers)");

    Texture2D Load(string path)
        => GD.Load<Texture2D>(path);

    void AddLayer(string dir)
    {
        foreach (var path in DataFiles.FilesRecursive(dir))
        {
            if (path.GetExtension() == "import") continue;
            var rel = path.Substring(dir.Length + 1);
            _byRel[rel.TrimSuffix($".{rel.GetExtension()}")] = path;
        }
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
