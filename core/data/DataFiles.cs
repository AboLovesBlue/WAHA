using Godot;
using System;
using System.Collections.Generic;

namespace WAHA;

public static class DataFiles
{
    public static Godot.Collections.Dictionary LoadFile(string path)
    {
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read)
            ?? throw new InvalidOperationException($"Missing data file: {path}");
        return Json.ParseString(file.GetAsText()).AsGodotDictionary();
    }

    public static List<string> JsonFiles(string dir)
    {
        var access = DirAccess.Open(dir)
            ?? throw new InvalidOperationException($"Missing data folder: {dir}");
        var files = new List<string>();
        access.ListDirBegin();
        var name = access.GetNext();
        while (name.Length > 0)
        {
            if (!access.CurrentIsDir() && name.GetExtension() == "json")
                files.Add($"{dir}/{name}");
            name = access.GetNext();
        }
        access.ListDirEnd();
        return files;
    }

    public static List<string> FilesRecursive(string dir)
    {
        var files = new List<string>();
        var access = DirAccess.Open(dir);
        if (access == null) return files; // asset layers are optional; an absent folder is an empty layer

        access.ListDirBegin();
        var name = access.GetNext();
        while (name.Length > 0)
        {
            var path = $"{dir}/{name}";
            if (access.CurrentIsDir())
            {
                if (!name.StartsWith(".")) files.AddRange(FilesRecursive(path));
            }
            else files.Add(path);
            name = access.GetNext();
        }
        access.ListDirEnd();
        return files;
    }
}
