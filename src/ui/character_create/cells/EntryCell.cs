using Godot;
using System;

namespace WAHA;

public interface EntryCell
{
    Button Build(ScreenData screen, SelectionEntry entry);
}
