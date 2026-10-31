using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WAHA;

// sprite entries are keyed by the asset id whose texture they animate; a screen
// element that wants the animation looks the id up against GameDB.Sprites
public static class SpriteLoader
{
    public static Dictionary<string, SpriteData> Load(string dir)
    {
        var dict = new Dictionary<string, SpriteData>();
        foreach (var path in DataFiles.FilesRecursive(dir).Where(p => p.GetExtension() == "json"))
            foreach (var (key, value) in DataFiles.LoadFile(path))
            {
                var sprite = value.AsGodotDictionary();
                dict[key.AsString()] = new SpriteData(
                    DataFiles.Str(sprite, "texture"), DataFiles.Vec2(sprite, "frame_size"),
                    DataFiles.Int(sprite, "row"), DataFiles.Int(sprite, "columns"),
                    DataFiles.Int(sprite, "fps"), DataFiles.Bool(sprite, "loop"));
            }
        return dict;
    }
}
