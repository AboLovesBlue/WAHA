using Godot;
using System.Collections.Generic;

namespace WAHA;

public record ThemeData(string Font, int Padding, int Gutter, Dictionary<string, PartData> Parts);
