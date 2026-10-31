using Godot;

namespace WAHA;

public class ColorCell : EntryCell
{
    readonly Texture2D character;

    public ColorCell(ScreenData screen, Texture2D character)
    {
        this.character = character;
    }

    public Button Build(ScreenData screen, SelectionEntry entry)
    {
        var layout = screen.Layout;
        var button = new Button
        {
            ToggleMode = true,
            CustomMinimumSize = layout.EntrySize,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        };
        Skin(screen, button);
        UiId.Tag(button, screen, "cell");

        var inset = ContentInset(screen);
        UiId.Tag(inset, screen, "cell_inset");
        var image = new TextureRect
        {
            // every color cell previews the character in its color; the unpicked custom slot stays white
            Texture = character,
            Modulate = entry.Color.Length > 0 ? new Color(entry.Color) : Colors.White,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.Scale,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
        };
        UiId.Tag(image, screen, "cell_image");
        inset.AddChild(image);
        button.AddChild(inset);
        return button;
    }

    static MarginContainer ContentInset(ScreenData screen)
    {
        var frame = screen.Theme.Parts["entry"].Frame;
        var inset = new MarginContainer
        {
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        inset.AddThemeConstantOverride("margin_left", frame.X);
        inset.AddThemeConstantOverride("margin_top", frame.Y);
        inset.AddThemeConstantOverride("margin_right", frame.Z);
        inset.AddThemeConstantOverride("margin_bottom", frame.W);
        inset.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        return inset;
    }

    static void Skin(ScreenData screen, Button button)
    {
        var normal = UiSkin.Style(screen, "entry");
        if (normal != null) button.AddThemeStyleboxOverride("normal", normal);
        var selected = UiSkin.Style(screen, "entry_selected");
        if (selected != null)
        {
            button.AddThemeStyleboxOverride("pressed", selected);
            button.AddThemeStyleboxOverride("hover", selected);
        }
        // one canvas item draws both cell states, so entry and entry_selected share one filter
        UiSkin.ApplyFilter(button, screen, "entry");
    }
}
