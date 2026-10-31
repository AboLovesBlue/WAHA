using System.Collections.Generic;

namespace WAHA;

public record RaceData(string NameKey, string Icon, Dictionary<string, List<SelectionEntry>> Fields);
