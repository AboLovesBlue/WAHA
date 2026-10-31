using Godot;
using System;
using System.Collections.Generic;

namespace WAHA;

public static class ScreenLoader
{
    public static Dictionary<string, ScreenData> Load(string dir)
    {
        var dict = new Dictionary<string, ScreenData>();
        foreach (var folder in DataFiles.SubFolders(dir))
        {
            var screenId = folder.GetFile();
            dict[screenId] = Screen(screenId, folder);
        }
        return dict;
    }

    static ScreenData Screen(string screenId, string folder)
    {
        TitleData? title = null;
        ThemeData? theme = null;
        ScreenLayout? layout = null;
        string background = "";
        List<ButtonData> buttons = new();
        List<TabData> tabs = new();
        Dictionary<string, string> ids = new();

        foreach (var path in DataFiles.JsonFiles(folder))
        {
            var part = DataFiles.LoadFile(path);
            switch (path.GetFile().GetBaseName())
            {
                case "title":
                    title = new TitleData(DataFiles.Str(part, "key"), DataFiles.OptInt(part, "font_size"));
                    break;
                case "theme":
                    theme = ThemeLoader.Parse(part);
                    break;
                case "background":
                    background = DataFiles.Str(part, "texture");
                    break;
                case "buttons":
                    buttons = Buttons(part, screenId);
                    break;
                case "layout":
                    layout = Layout(part);
                    break;
                case "tabs":
                    tabs = Tabs(part, screenId);
                    break;
                case "ids":
                    ids = Ids(part);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Unknown screen part file: {path} (parts: title, theme, background, buttons, layout, tabs, ids)");
            }
        }

        if (theme == null)
            throw new InvalidOperationException($"Screen {screenId} is missing its theme part (theme.json)");

        foreach (var id in ids.Values)
            if (!id.StartsWith($"ui.{screenId}."))
                throw new InvalidOperationException(
                    $"Ui id must start with \"ui.{screenId}.\": {id} (screen {screenId})");

        return new ScreenData(screenId, title, background, theme, buttons, layout, tabs, ids);
    }

    static Dictionary<string, string> Ids(Godot.Collections.Dictionary part)
    {
        var ids = new Dictionary<string, string>();
        if (!part.ContainsKey("ids")) return ids;
        foreach (var (key, value) in part["ids"].AsGodotDictionary())
            ids[key.AsString()] = value.AsString();
        return ids;
    }

    static List<ButtonData> Buttons(Godot.Collections.Dictionary part, string screenId)
    {
        var buttons = new List<ButtonData>();
        foreach (var buttonVariant in part["buttons"].AsGodotArray())
        {
            var b = buttonVariant.AsGodotDictionary();
            var id = b["id"].AsString();
            if (DataFiles.Str(b, "key").Length == 0 && DataFiles.Str(b, "texture").Length == 0)
                throw new InvalidOperationException($"Button {id} (screen {screenId}) needs either a key or a texture");
            var align = DataFiles.Str(b, "align");
            if (align != "left" && align != "right")
                throw new InvalidOperationException(
                    $"Button {id} (screen {screenId}) align must be \"left\" or \"right\"");
            buttons.Add(new ButtonData(id, DataFiles.Str(b, "key"), DataFiles.Str(b, "action"),
                DataFiles.Str(b, "texture"), align, DataFiles.OptInt(b, "font_size")));
        }
        return buttons;
    }

    static List<TabData> Tabs(Godot.Collections.Dictionary part, string screenId)
    {
        var tabs = new List<TabData>();
        foreach (var tabVariant in part["tabs"].AsGodotArray())
        {
            var t = tabVariant.AsGodotDictionary();
            var cell = DataFiles.Str(t, "cell");
            if (cell.Length == 0)
                throw new InvalidOperationException($"Tab {t["id"].AsString()} (screen {screenId}) is missing its cell id");
            tabs.Add(new TabData(t["id"].AsString(), t["key"].AsString(), t["source"].AsString(),
                cell, t.ContainsKey("custom") && t["custom"].AsBool(), t.ContainsKey("tint") && t["tint"].AsBool()));
        }
        return tabs;
    }

    static ScreenLayout Layout(Godot.Collections.Dictionary part)
    {
        var bar = new List<string>();
        if (part.ContainsKey("bar"))
            foreach (var idVariant in part["bar"].AsGodotArray())
                bar.Add(idVariant.AsString());
        return new ScreenLayout(
            DataFiles.Vec2(part, "size"), DataFiles.Vec2(part, "pane_size"), DataFiles.Int(part, "columns"),
            DataFiles.Vec2(part, "entry_size"), DataFiles.Vec2(part, "icon_size"),
            DataFiles.Hex(part, "tint"),
            DataFiles.Str(part, "picker_shape"), DataFiles.Str(part, "picker_color"),
            DataFiles.Int(part, "tab_font_size"), DataFiles.Float(part, "tab_dim"),
            DataFiles.Int(part, "current_font_size"),
            DataFiles.Int(part, "head_height"), DataFiles.Int(part, "head_inset"),
            DataFiles.Str(part, "character"), bar);
    }
}
