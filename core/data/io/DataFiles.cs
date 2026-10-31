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

    public static Godot.Collections.Array LoadArray(string path)
    {
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read)
            ?? throw new InvalidOperationException($"Missing data file: {path}");
        return Json.ParseString(file.GetAsText()).AsGodotArray();
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

    public static List<string> SubFolders(string dir)
    {
        var access = DirAccess.Open(dir)
            ?? throw new InvalidOperationException($"Missing data folder: {dir}");
        var folders = new List<string>();
        access.ListDirBegin();
        var name = access.GetNext();
        while (name.Length > 0)
        {
            if (access.CurrentIsDir() && !name.StartsWith(".")) folders.Add($"{dir}/{name}");
            name = access.GetNext();
        }
        access.ListDirEnd();
        return folders;
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

    public static string Str(Godot.Collections.Dictionary dict, string field)
        => dict.ContainsKey(field) ? dict[field].AsString() : "";

    public static int Int(Godot.Collections.Dictionary dict, string field)
        => dict.ContainsKey(field) ? dict[field].AsInt32()
           : throw new InvalidOperationException($"Missing integer field: {field}");

    public static int OptInt(Godot.Collections.Dictionary dict, string field)
        => dict.ContainsKey(field) ? Int(dict, field) : 0;

    public static Vector2I Vec2(Godot.Collections.Dictionary dict, string field)
        => dict.ContainsKey(field) ? Vec(dict[field].AsGodotArray(), field)
           : throw new InvalidOperationException($"Missing vector field: {field}");

    static Vector2I Vec(Godot.Collections.Array array, string field)
        => array.Count == 2 ? new Vector2I(array[0].AsInt32(), array[1].AsInt32())
           : throw new InvalidOperationException($"Field {field} must be [x, y]");

    public static Color Hex(Godot.Collections.Dictionary dict, string field)
        => dict.ContainsKey(field) ? new Color(dict[field].AsString())
           : throw new InvalidOperationException($"Missing color field: {field}");

    public static bool Bool(Godot.Collections.Dictionary dict, string field)
        => dict.ContainsKey(field) ? dict[field].AsBool()
           : throw new InvalidOperationException($"Missing bool field: {field}");

    public static float Float(Godot.Collections.Dictionary dict, string field)
        => dict.ContainsKey(field) ? dict[field].AsSingle()
           : throw new InvalidOperationException($"Missing float field: {field}");

    public static Vector4I Edges(Godot.Collections.Dictionary dict, string field)
        => dict.ContainsKey(field) ? Edge(dict[field].AsGodotArray(), field)
           : throw new InvalidOperationException($"Missing edges field: {field}");

    static Vector4I Edge(Godot.Collections.Array array, string field)
        => array.Count == 4
            ? new Vector4I(array[0].AsInt32(), array[1].AsInt32(), array[2].AsInt32(), array[3].AsInt32())
            : throw new InvalidOperationException($"Field {field} must be [left, top, right, bottom]");
}
