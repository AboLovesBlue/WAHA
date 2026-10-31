using Godot;

namespace WAHA;

public record SpriteData(string Texture, Vector2I FrameSize, int Row, int Columns, int Fps, bool Loop);
