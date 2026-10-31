using System.Collections.Generic;

namespace WAHA;

public record ScreenData(string ScreenId, TitleData? Title, string Background, ThemeData Theme,
    List<ButtonData> Buttons, ScreenLayout Layout, List<TabData> Tabs,
    Dictionary<string, string> Ids);
