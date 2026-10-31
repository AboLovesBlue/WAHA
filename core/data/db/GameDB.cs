using Godot;
using System.Collections.Generic;

namespace WAHA;

public partial class GameDB : Node
{
    public static GameDB Instance { get; private set; } = null!;

    public Dictionary<string, ScreenData> Ui { get; private set; } = new();
    public Dictionary<string, RaceData> Races { get; private set; } = new();
    public Dictionary<string, SpriteData> Sprites { get; private set; } = new();

    public override void _Ready()
    {
        Instance = this;
        Ui = ScreenLoader.Load("res://data/ui");
        Races = RaceLoader.Load("res://data/races");
        Sprites = SpriteLoader.Load("res://data/sprites");
    }
}
