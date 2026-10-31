using Godot;
using System.Collections.Generic;

namespace WAHA;

public record ScreenLayout(Vector2I Size, Vector2I PaneSize, int Columns, Vector2I EntrySize,
    Vector2I IconSize, Color Tint, string PickerShape, string PickerColor, int TabFontSize, float TabDim,
    int CurrentFontSize, int HeadHeight, int HeadInset, string Character, List<string> Bar);
