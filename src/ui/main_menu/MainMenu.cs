using Godot;
using System;

namespace WAHA;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        var menu = GameDB.Instance.Ui["main_menu"];

        Theme = UiSkin.FontOf(menu);

        if (menu.Background.Length > 0)
        {
            var back = new TextureRect
            {
                Texture = Assets.Instance.Tex(menu.Background),
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            };
            back.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            UiId.Tag(back, menu, "background");
            AddChild(back);
            MoveChild(back, 0);
        }

        var title = GetNode<Label>("Center/Box/Title");
        if (menu.Title != null)
        {
            title.Text = Loc.Instance.Tr(menu.Title.Key);
            title.AddThemeFontSizeOverride("font_size", menu.Title.FontSize);
            UiId.Tag(title, menu, "title");
        }

        var box = GetNode<VBoxContainer>("Center/Box/Buttons");
        UiId.Tag(box, menu, "buttons");
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
            button.Pressed += () => Actions.Run(this, buttonData.Action);
            UiId.Tag(button, menu, buttonData.Id);
            box.AddChild(button);
        }
    }
}
