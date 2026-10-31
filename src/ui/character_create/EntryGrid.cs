using Godot;
using System;
using System.Collections.Generic;

namespace WAHA;

public class EntryGrid
{
	readonly ScrollContainer scroll = new();
	readonly GridContainer grid = new();
	readonly ScreenData screen;
	readonly EntryCellFactory cells;

	public Control Node => scroll;
	public event System.Action<SelectionEntry> EntryPicked;

	public EntryGrid(EntryCellFactory cells, ScreenData screen)
	{
		this.cells = cells;
		this.screen = screen;
		var layout = screen.Layout
			?? throw new InvalidOperationException("Entry grid requires a screen layout block");
		var gutter = screen.Theme.Gutter;
		grid.Columns = layout.Columns;
		grid.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill; // columns stretch so cells fill the content area edge to edge
		grid.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter; // sparse tabs center their rows so leftover space splits evenly top/bottom
		grid.AddThemeConstantOverride("h_separation", gutter);
		grid.AddThemeConstantOverride("v_separation", gutter);
		UiId.Tag(grid, screen, "grid");

		scroll.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		scroll.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		UiId.Tag(scroll, screen, "grid_scroll");
		scroll.AddChild(grid);
	}

	public void Show(List<SelectionEntry> entries, string cellId, string selectedId)
	{
		foreach (Node child in grid.GetChildren())
			child.QueueFree();

		foreach (var entry in entries)
		{
			var button = cells.Build(screen, cellId, entry);
			button.ButtonPressed = entry.Id == selectedId;
			button.Pressed += () => Pick(button, entry);
			grid.AddChild(button);
		}
	}

	void Pick(Button pressed, SelectionEntry entry)
	{
		foreach (Button other in grid.GetChildren())
			other.ButtonPressed = other == pressed;
		EntryPicked?.Invoke(entry);
	}
}
