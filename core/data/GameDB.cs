using Godot;
using System.Collections.Generic;

namespace WAHA;

public partial class GameDB : Node
{
    public static GameDB Instance { get; private set; } = null!;

    public record ScreenData(string TitleKey, string Background, List<ButtonData> Buttons);
    public record ButtonData(string Id, string Key, string Action, string Texture);

    public Dictionary<string, ScreenData> Ui { get; private set; } = new();

    public override void _Ready()
    {
        Instance = this;
        Ui = LoadScreens("res://data/ui");
    }

    Dictionary<string, ScreenData> LoadScreens(string dir)
    {
        var dict = new Dictionary<string, ScreenData>();
        foreach (var path in DataFiles.JsonFiles(dir))
        {
            var screen = DataFiles.LoadFile(path);
            var buttons = new List<ButtonData>();
            foreach (var buttonVariant in screen["buttons"].AsGodotArray())
            {
                var b = buttonVariant.AsGodotDictionary();
                buttons.Add(new ButtonData(
                    b["id"].AsString(), b["key"].AsString(), b["action"].AsString(), Str(b, "texture")));
            }
            dict[path.GetFile().GetBaseName()] =
                new ScreenData(screen["title_key"].AsString(), Str(screen, "background"), buttons);
        }
        return dict;
    }

    static string Str(Godot.Collections.Dictionary dict, string field)
        => dict.ContainsKey(field) ? dict[field].AsString() : "";
}
