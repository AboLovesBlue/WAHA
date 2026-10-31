using Godot;
using System;
using System.Collections.Generic;

namespace WAHA;

public static class RaceLoader
{
    public static Dictionary<string, RaceData> Load(string dir)
    {
        var dict = new Dictionary<string, RaceData>();
        foreach (var folder in DataFiles.SubFolders(dir))
        {
            var raceId = folder.GetFile();
            dict[raceId] = Read(folder, raceId);
        }
        return dict;
    }

    static RaceData Read(string folder, string raceId)
    {
        string key = "";
        string texture = "";
        var hasInfo = false;
        var fields = new Dictionary<string, List<SelectionEntry>>();
        foreach (var path in DataFiles.JsonFiles(folder))
        {
            var part = path.GetFile().GetBaseName();
            if (part == "info")
            {
                var info = DataFiles.LoadFile(path);
                key = DataFiles.Str(info, "key");
                texture = DataFiles.Str(info, "texture");
                hasInfo = true;
            }
            else
                fields[part] = Entries(DataFiles.LoadArray(path), part, path);
        }
        if (!hasInfo)
            throw new InvalidOperationException($"Race {raceId} is missing its info part (info.json)");
        return new RaceData(key, texture, fields);
    }

    static List<SelectionEntry> Entries(Godot.Collections.Array array, string field, string path)
    {
        var list = new List<SelectionEntry>();
        foreach (var item in array)
        {
            var d = item.AsGodotDictionary();
            if (!d.ContainsKey("id"))
                throw new InvalidOperationException($"Race entry missing id in field {field}: {path}");
            list.Add(new SelectionEntry(
                d["id"].AsString(), DataFiles.Str(d, "key"), DataFiles.Str(d, "icon"), DataFiles.Str(d, "color")));
        }
        return list;
    }
}
