using Godot;

namespace WAHA;

public class IconCell : EntryCell
{
    readonly Texture2D placeholder;

    public IconCell(ScreenData screen, Texture2D placeholder)
    {
        this.placeholder = placeholder;
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
            Texture = entry.Icon.Length > 0 ? Assets.Instance.Tex(entry.Icon) : placeholder,
            Modulate = entry.Color.Length > 0 ? new Color(entry.Color) : layout.Tint,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.Scale,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill,
        };
        UiId.Tag(image, screen, "cell_image");
        inset.AddChild(image);
        button.AddChild(inset);
        return button;
    }

    // anchored children ignore the stylebox content margins, so the inset is applied
    // explicitly with the same frame values the entry style itself uses
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
