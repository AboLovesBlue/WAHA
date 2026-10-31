using Godot;
using System;
using System.Collections.Generic;

namespace WAHA;

public static class ThemeLoader
{
    public static ThemeData Parse(Godot.Collections.Dictionary theme)
    {
        var parts = new Dictionary<string, PartData>();
        if (theme.ContainsKey("parts"))
            foreach (var (key, value) in theme["parts"].AsGodotDictionary())
            {
                var part = value.AsGodotDictionary();
                parts[key.AsString()] = new PartData(
                    DataFiles.Str(part, "texture"), DataFiles.Edges(part, "frame"), DataFiles.Hex(part, "bg"),
                    DataFiles.Edges(part, "border"), DataFiles.Hex(part, "border_color"), DataFiles.Edges(part, "corners"),
                    Stretch<StyleBoxTexture.AxisStretchMode>(part, "h"),
                    Stretch<StyleBoxTexture.AxisStretchMode>(part, "v"),
                    Filter(part),
                    part.ContainsKey("exact") && part["exact"].AsBool());
            }
        return new ThemeData(DataFiles.Str(theme, "font"), DataFiles.Int(theme, "padding"),
            DataFiles.Int(theme, "gutter"), parts);
    }

    static T Stretch<T>(Godot.Collections.Dictionary part, string axis) where T : struct, Enum
    {
        var field = DataFiles.Str(part, $"stretch_{axis}");
        var name = field switch
        {
            "" => "Stretch",
            "stretch" => "Stretch",
            "tile" => "Tile",
            _ => throw new InvalidOperationException(
                $"Theme part stretch_{axis} must be \"stretch\" or \"tile\""),
        };
        return (T)Enum.Parse(typeof(T), name);
    }

    static CanvasItem.TextureFilterEnum Filter(Godot.Collections.Dictionary part)
        => DataFiles.Str(part, "filter") switch
        {
            "" => CanvasItem.TextureFilterEnum.Linear,
            "linear" => CanvasItem.TextureFilterEnum.Linear,
            "nearest" => CanvasItem.TextureFilterEnum.Nearest,
            _ => throw new InvalidOperationException(
                $"Theme part filter must be \"linear\" or \"nearest\""),
        };
}
