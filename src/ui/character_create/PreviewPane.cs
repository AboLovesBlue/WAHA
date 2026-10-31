using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WAHA;

public class PreviewPane
{
    readonly ScreenData screen;
    readonly ScreenLayout layout;
    readonly Texture2D character;
    readonly TextureRect preview = new();
    readonly Timer timer = new();
    List<ImageTexture> frames = null!;
    SpriteData sprite = null!;
    int frameIndex;

    public Control Node { get; }

    public PreviewPane(ScreenData screen)
    {
        this.screen = screen;
        layout = screen.Layout
            ?? throw new InvalidOperationException("Preview pane requires a screen layout block");
        character = Assets.Instance.Tex(layout.Character);

        var frame = new PanelContainer();
        frame.CustomMinimumSize = layout.PaneSize;
        frame.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        frame.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        var style = UiSkin.Style(screen, "preview");
        if (style != null) frame.AddThemeStyleboxOverride("panel", style);
        UiSkin.ApplyFilter(frame, screen, "preview");
        UiId.Tag(frame, screen, "preview_frame");

        var box = new VBoxContainer();
        box.AddThemeConstantOverride("separation", screen.Theme.Gutter);
        UiId.Tag(box, screen, "preview_box");
        frame.AddChild(box);

        preview.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        preview.Texture = character;
        preview.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        preview.StretchMode = TextureRect.StretchModeEnum.Scale;
        preview.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        UiId.Tag(preview, screen, "preview_image");
        box.AddChild(preview);
        box.AddChild(BuildBar());
        UiId.Tag(timer, screen, "preview_timer");

        timer.Timeout += NextFrame;
        frame.AddChild(timer);
        Node = frame;
    }

    public event Action<string> ActionChosen = null!;

    // plain clickable words at the bottom of the preview, no button chrome
    Control BuildBar()
    {
        var gutter = screen.Theme.Gutter;
        List<Control> left = new(), right = new();
        foreach (var id in layout.Bar)
        {
            var entry = screen.Buttons.FirstOrDefault(b => b.Id == id)
                ?? throw new InvalidOperationException(
                    $"Bar layout id \"{id}\" (screen {screen.ScreenId}) matches no button in buttons.json");
            Control word;
            if (entry.Texture.Length > 0)
                word = new TextureRect
                {
                    Texture = Assets.Instance.Tex(entry.Texture),
                    CustomMinimumSize = layout.IconSize,
                    ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                    StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                };
            else
            {
                var label = new Label { Text = Loc.Instance.Tr(entry.Key) };
                if (entry.FontSize > 0) label.AddThemeFontSizeOverride("font_size", entry.FontSize);
                word = label;
            }
            word.MouseFilter = Control.MouseFilterEnum.Stop;
            word.GuiInput += @event =>
            {
                if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
                    ActionChosen.Invoke(entry.Action);
            };
            UiId.Tag(word, screen, entry.Id);
            if (entry.Align == "left") left.Add(word);
            else right.Add(word);
        }
        var bar = new HBoxContainer();
        bar.AddThemeConstantOverride("separation", gutter);
        UiId.Tag(bar, screen, "preview_bar");
        foreach (var word in left) bar.AddChild(word);
        if (right.Count > 0)
        {
            var gap = new Control();
            gap.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            bar.AddChild(gap);
            foreach (var word in right) bar.AddChild(word);
        }
        return bar;
    }

    public void Show(SelectionEntry entry, Color filter)
    {
        preview.Modulate = filter;
        Stop();
        if (GameDB.Instance.Sprites.TryGetValue(entry.Icon, out var data))
            Play(data, entry.Icon);
        else
            preview.Texture = entry.Icon.Length > 0 ? Assets.Instance.Tex(entry.Icon) : character;
    }

    public void ApplyFilter(Color filter)
        => preview.Modulate = filter;

    void Play(SpriteData data, string spriteId)
    {
        sprite = data;
        preview.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        // independent crops, not atlas regions: TextureRect mis-renders atlas region textures
        var sheet = Assets.Instance.Tex(data.Texture).GetImage();
        sheet.Convert(Image.Format.Rgba8);
        frames = new List<ImageTexture>();
        for (var i = 0; i < data.Columns; i++)
        {
            var cut = sheet.GetRegion(
                new Rect2I(i * data.FrameSize.X, data.Row * data.FrameSize.Y, data.FrameSize.X, data.FrameSize.Y));
            frames.Add(ImageTexture.CreateFromImage(cut));
        }
        frameIndex = 0;
        preview.Texture = frames[0];
        timer.Stop();
        timer.WaitTime = 1f / data.Fps;
        timer.Start();
    }

    void Stop()
    {
        timer.Stop();
        frames = null!;
        sprite = null!;
    }

    void NextFrame()
    {
        if (!sprite.Loop && frameIndex == frames.Count - 1)
        {
            timer.Stop();
            return;
        }
        frameIndex = (frameIndex + 1) % frames.Count;
        preview.Texture = frames[frameIndex];
    }
}
