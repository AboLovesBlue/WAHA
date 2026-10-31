using Godot;

namespace WAHA;

public record PartData(
    string Texture, Vector4I Frame, Color Bg, Vector4I Border, Color BorderColor, Vector4I Corners,
    StyleBoxTexture.AxisStretchMode HStretch, StyleBoxTexture.AxisStretchMode VStretch,
    CanvasItem.TextureFilterEnum Filter, bool Exact);
