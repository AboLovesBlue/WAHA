using Godot;
using System;

namespace WAHA;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        var menu = GameDB.Instance.Ui["main_menu"];

        if (menu.Background.Length > 0)
        {
            var back = new TextureRect
            {
                Texture = Assets.Instance.Tex(menu.Background),
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            };
            back.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            AddChild(back);
            MoveChild(back, 0);
        }

        GetNode<Label>("Center/Box/Title").Text = Loc.Instance.Tr(menu.TitleKey);

        var box = GetNode<VBoxContainer>("Center/Box/Buttons");
        foreach (var buttonData in menu.Buttons)
        {
            var button = new Button { Text = Loc.Instance.Tr(buttonData.Key) };
            if (buttonData.Texture.Length > 0)
            {
                var style = new StyleBoxTexture { Texture = Assets.Instance.Tex(buttonData.Texture) };
                button.AddThemeStyleboxOverride("normal", style);
                button.AddThemeStyleboxOverride("hover", style);
                button.AddThemeStyleboxOverride("pressed", style);
            }
            button.Pressed += () => OnPressed(buttonData);
            box.AddChild(button);
        }
    }

    void OnPressed(GameDB.ButtonData buttonData)
    {
        switch (buttonData.Action)
        {
            case "new_game":
                GD.Print("New game started");
                break;
            case "exit":
                GetTree().Quit();
                break;
            default:
                throw new InvalidOperationException($"Unknown menu action: {buttonData.Action} (id {buttonData.Id})");
        }
    }
}
