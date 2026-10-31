using System;
using System.Collections.Generic;
using System.Linq;

namespace WAHA;

public class EntryResolver
{
	public const string Races = "races";
	public const string Custom = "custom";

	readonly Dictionary<string, RaceData> races;
	readonly SelectionState state;
	readonly ScreenLayout layout;

	public EntryResolver(Dictionary<string, RaceData> races, SelectionState state, ScreenLayout layout)
	{
		this.races = races;
		this.state = state;
		this.layout = layout;
	}

	public List<SelectionEntry> Entries(TabData tab)
	{
		// copy: the lists stored in RaceData are shared data and must be treated read-only
		var entries = tab.Source == Races ? RaceEntries() : new List<SelectionEntry>(Field(tab.Source));
		if (tab.Custom)
			entries.Insert(0, new SelectionEntry(Custom, "", layout.Character, state.Custom(tab.Source)));
		return entries;
	}

	public SelectionEntry Current(TabData tab)
	{
		var entries = Entries(tab);
		if (entries.Count == 0)
			throw new InvalidOperationException($"Tab {tab.Id} has no source entries: {tab.Source}");
		var id = state.Pick(tab.Source);
		return id.Length > 0 ? entries.FirstOrDefault(e => e.Id == id) ?? entries[0] : entries[0];
	}

	List<SelectionEntry> RaceEntries()
		=> races.Keys.OrderBy(id => id)
			.Select(id => new SelectionEntry(id, races[id].NameKey, races[id].Icon, ""))
			.ToList();

	List<SelectionEntry> Field(string source)
	{
		var race = Race();
		if (!race.Fields.TryGetValue(source, out var entries))
			throw new InvalidOperationException(
				$"Unknown tab source: {source}. Race fields: {string.Join(", ", race.Fields.Keys)}");
		return entries;
	}

	RaceData Race()
	{
		var id = state.Pick(Races) ?? throw new InvalidOperationException("No race selected");
		return races.TryGetValue(id, out var race) ? race
			: throw new InvalidOperationException($"Unknown race: {id}");
	}
}
