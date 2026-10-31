using Godot;
using System;
using System.Linq;

namespace WAHA;

public partial class CharacterCreate : Control
{
	ScreenData screen = null!;
	SelectionState state = null!;
	EntryResolver resolver = null!;
	TabBar tabBar = null!;
	EntryGrid grid = null!;
	PreviewPane preview = null!;
	ColorPickerPopup picker = null!;
	TabData currentTab = null!;
	string pickingSource = "";

	public override void _Ready()
	{
		screen = GameDB.Instance.Ui["character_create"];
		var layout = screen.Layout
			?? throw new InvalidOperationException("Screen character_create has no layout block");
		var races = GameDB.Instance.Races;
		if (races.Count == 0)
			throw new InvalidOperationException("No races defined: data/races is empty");

		state = new SelectionState();
		resolver = new EntryResolver(races, state, layout);
		state.Select(EntryResolver.Races, races.Keys.First());

		Theme = UiSkin.FontOf(screen);

		picker = new ColorPickerPopup(layout.PickerShape, layout.PickerColor, screen);
		picker.ColorCommitted += OnColorCommitted;
		AddChild(picker.Node);
		UiId.Tag(picker.Node, screen, "color_picker");
		Actions.ScreenAction += OnScreenAction;
		TreeExited += () => Actions.ScreenAction -= OnScreenAction;

		BuildShell(layout);
		preview.ActionChosen += action => Actions.Run(this, action);
		tabBar.Select(screen.Tabs[0]);
	}

	// any tab flagged tint-this provides the persistent color filter: changing race or sex keeps the skin tone applied
	Color FilterColor()
	{
		var tintTab = screen.Tabs.FirstOrDefault(t => t.Tint);
		if (tintTab == null) return new Color(1, 1, 1, 1);
		var hex = resolver.Current(tintTab).Color;
		return hex.Length > 0 ? new Color(hex) : new Color(1, 1, 1, 1);
	}

	void BuildShell(ScreenLayout layout)
	{
		var center = new CenterContainer();
		center.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		AddChild(center);

		var panel = new PanelContainer();
		panel.CustomMinimumSize = layout.Size;
		var panelStyle = UiSkin.Style(screen, "panel");
		if (panelStyle != null) panel.AddThemeStyleboxOverride("panel", panelStyle);
		UiSkin.ApplyFilter(panel, screen, "panel");
		UiId.Tag(panel, screen, "panel");
		center.AddChild(panel);

		var gutter = screen.Theme.Gutter;
		var inset = new MarginContainer();
		inset.AddThemeConstantOverride("margin_left", screen.Theme.Padding);
		inset.AddThemeConstantOverride("margin_top", screen.Theme.Padding);
		inset.AddThemeConstantOverride("margin_right", screen.Theme.Padding);
		inset.AddThemeConstantOverride("margin_bottom", screen.Theme.Padding);
		UiId.Tag(inset, screen, "inset");
		panel.AddChild(inset);

		var split = new HBoxContainer();
		split.AddThemeConstantOverride("separation", gutter);
		UiId.Tag(split, screen, "split");
		inset.AddChild(split);

		preview = new PreviewPane(screen);
		split.AddChild(preview.Node);

		var chooser = new VBoxContainer();
		chooser.CustomMinimumSize = layout.PaneSize;
		chooser.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		chooser.AddThemeConstantOverride("separation", 0); // tabs sit directly on the content panel so a selected tab merges into it
		UiId.Tag(chooser, screen, "chooser");
		tabBar = new TabBar(screen.Tabs, screen);
		tabBar.TabSelected += OnTab;
		var head = new MarginContainer(); // the head is slightly narrower than the body below it
		head.SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		head.CustomMinimumSize = new Vector2(0, layout.HeadHeight);
		head.AddThemeConstantOverride("margin_left", layout.HeadInset);
		head.AddThemeConstantOverride("margin_right", layout.HeadInset);
		head.AddChild(tabBar.Node);
		UiId.Tag(head, screen, "head");
		chooser.AddChild(head);
		grid = new EntryGrid(new EntryCellFactory(screen), screen);
		grid.EntryPicked += OnEntryPicked;
		var content = new PanelContainer();
		var contentStyle = UiSkin.Style(screen, "content");
		if (contentStyle != null) content.AddThemeStyleboxOverride("panel", contentStyle);
		UiSkin.ApplyFilter(content, screen, "content");
		content.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		content.AddChild(grid.Node);
		UiId.Tag(content, screen, "content");
		chooser.AddChild(content);
		split.AddChild(chooser);
	}

	void OnScreenAction(Control source, string action)
	{
		tabBar.Cycle(action switch
		{
			"tab:prev" => -1,
			"tab:next" => 1,
			_ => throw new InvalidOperationException($"Unknown tab action: {action}"),
		});
	}

	void OnTab(TabData tab)
	{
		currentTab = tab;
		grid.Show(resolver.Entries(tab), tab.Cell, resolver.Current(tab).Id);
		if (!tab.Tint) preview.Show(resolver.Current(tab), FilterColor());
		else preview.ApplyFilter(FilterColor());
	}

	void OnEntryPicked(SelectionEntry entry)
	{
		state.Select(currentTab.Source, entry.Id);
		if (entry.Id == EntryResolver.Custom)
		{
			pickingSource = currentTab.Source;
			picker.Open(state.Custom(currentTab.Source));
			return; // custom stays unfiltered until a color is committed
		}
		if (!currentTab.Tint) preview.Show(resolver.Current(currentTab), FilterColor());
		else preview.ApplyFilter(FilterColor());
	}

	void OnColorCommitted(string hex)
	{
		state.SetCustom(pickingSource, hex);
		preview.ApplyFilter(FilterColor());
	}
}
