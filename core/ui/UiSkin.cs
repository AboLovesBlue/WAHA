using Godot;
using System;

namespace WAHA;

public static class UiSkin
{
    // styleboxes draw through the control's own texture filter; a part opting into
    // "filter": "nearest" must set it on every control the part's style renders from
    public static CanvasItem.TextureFilterEnum FilterOf(ScreenData screen, string part)
    {
        if (!screen.Theme.Parts.TryGetValue(part, out var partData))
            throw new InvalidOperationException(
                $"Unknown theme part: {part} (screen {screen.ScreenId}, parts: {string.Join(", ", screen.Theme.Parts.Keys)})");
        return partData.Filter;
    }

    public static void ApplyFilter(Control control, ScreenData screen, string part)
        => control.TextureFilter = FilterOf(screen, part);

    public static Godot.Theme? FontOf(ScreenData screen)
    {
        var fontId = screen.Theme.Font;
        return fontId.Length == 0 ? null : new Godot.Theme { DefaultFont = Assets.Instance.Font(fontId) };
    }

    public static StyleBox Style(ScreenData screen, string part)
    {
        if (!screen.Theme.Parts.TryGetValue(part, out var partData))
            throw new InvalidOperationException(
                $"Unknown theme part: {part} (screen {screen.ScreenId}, parts: {string.Join(", ", screen.Theme.Parts.Keys)})");

        if (partData.Texture.Length > 0)
        {
            var texture = new StyleBoxTexture { Texture = Assets.Instance.Tex(partData.Texture) };
            if (!partData.Exact)
            {
                texture.TextureMarginLeft = partData.Frame.X;
                texture.TextureMarginTop = partData.Frame.Y;
                texture.TextureMarginRight = partData.Frame.Z;
                texture.TextureMarginBottom = partData.Frame.W;
                texture.AxisStretchHorizontal = partData.HStretch;
                texture.AxisStretchVertical = partData.VStretch;
            }
            texture.ContentMarginLeft = partData.Frame.X;
            texture.ContentMarginTop = partData.Frame.Y;
            texture.ContentMarginRight = partData.Frame.Z;
            texture.ContentMarginBottom = partData.Frame.W;
            return texture;
        }

        var flat = new StyleBoxFlat { BgColor = partData.Bg, BorderColor = partData.BorderColor };
        flat.BorderWidthLeft = partData.Border.X;
        flat.BorderWidthTop = partData.Border.Y;
        flat.BorderWidthRight = partData.Border.Z;
        flat.BorderWidthBottom = partData.Border.W;
        flat.CornerRadiusTopLeft = partData.Corners.X;
        flat.CornerRadiusTopRight = partData.Corners.Y;
        flat.CornerRadiusBottomRight = partData.Corners.Z;
        flat.CornerRadiusBottomLeft = partData.Corners.W;
        flat.ContentMarginLeft = partData.Frame.X;
        flat.ContentMarginTop = partData.Frame.Y;
        flat.ContentMarginRight = partData.Frame.Z;
        flat.ContentMarginBottom = partData.Frame.W;
        return flat;
    }
}
