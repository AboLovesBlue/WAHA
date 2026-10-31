using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WAHA;

// one wide tab bar: chevron left, previous section (dim), current section (big, centered),
// next section (dim), chevron right; the bar sits on the content panel it switches
public class TabBar
{
    readonly List<TabData> tabs;
    readonly TextureButton left = new();
    readonly TextureButton right = new();
    readonly Label prev = new();
    readonly Label current = new();
    readonly Label next = new();
    int index;

    public Control Node { get; }
    public event Action<TabData> TabSelected;

    public TabBar(List<TabData> tabs, ScreenData screen)
    {
        this.tabs = tabs;
        var layout = screen.Layout
            ?? throw new InvalidOperationException($"Screen {screen.ScreenId} has no layout block");

        left.TextureNormal = Assets.Instance.Tex(Button(screen, "previous").Texture);
        right.TextureNormal = Assets.Instance.Tex(Button(screen, "next").Texture);
        UiId.Tag(left, screen, "previous");
        UiId.Tag(right, screen, "next");
        foreach (var chevron in new[] { left, right })
        {
            chevron.CustomMinimumSize = layout.IconSize;
            chevron.IgnoreTextureSize = true;
            chevron.StretchMode = TextureButton.StretchModeEnum.KeepAspectCentered;
        }
        left.Pressed += () => Actions.Run(left, Button(screen, "previous").Action);
        right.Pressed += () => Actions.Run(right, Button(screen, "next").Action);

        foreach (var small in new[] { prev, next })
        {
            small.AddThemeFontSizeOverride("font_size", layout.TabFontSize);
            small.Modulate = new Color(1, 1, 1, layout.TabDim);
            small.VerticalAlignment = VerticalAlignment.Center;
            small.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        }
        UiId.Tag(prev, screen, "prev_word");
        UiId.Tag(next, screen, "next_word");
        prev.HorizontalAlignment = HorizontalAlignment.Left;
        next.HorizontalAlignment = HorizontalAlignment.Right;
        current.AddThemeFontSizeOverride("font_size", screen.Layout.CurrentFontSize);
        current.VerticalAlignment = VerticalAlignment.Center;
        current.HorizontalAlignment = HorizontalAlignment.Center;
        UiId.Tag(current, screen, "current_word");

        var strip = new HBoxContainer();
        strip.AddThemeConstantOverride("separation", screen.Theme.Gutter);
        UiId.Tag(strip, screen, "strip");
        strip.AddChild(left);
        strip.AddChild(prev);
        strip.AddChild(current);
        strip.AddChild(next);
        strip.AddChild(right);

        var panel = new PanelContainer();
        var style = UiSkin.Style(screen, "tab_bar");
        if (style != null) panel.AddThemeStyleboxOverride("panel", style);
        UiSkin.ApplyFilter(panel, screen, "tab_bar");
        UiId.Tag(panel, screen, "tab_bar");
        panel.AddChild(strip);
        Node = panel;
    }

    public void Select(TabData tab)
    {
        var i = tabs.IndexOf(tab);
        if (i < 0)
            throw new InvalidOperationException($"Unknown tab: {tab.Id}");
        index = i;
        current.Text = Loc.Instance.Tr(tab.Key);
        prev.Text = Loc.Instance.Tr(tabs[Wrap(index - 1)].Key);
        next.Text = Loc.Instance.Tr(tabs[Wrap(index + 1)].Key);
        TabSelected?.Invoke(tab);
    }

    public void Cycle(int delta) => Select(tabs[Wrap(index + delta)]);

    int Wrap(int i)
        => ((i % tabs.Count) + tabs.Count) % tabs.Count;

    static ButtonData Button(ScreenData screen, string id)
        => screen.Buttons.FirstOrDefault(b => b.Id == id)
           ?? throw new InvalidOperationException(
               $"Tab chevron \"{id}\" matches no button (screen {screen.ScreenId}) — add it to buttons.json");
}
