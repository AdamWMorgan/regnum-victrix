using Godot;
using System.Text.Json;

public static class ConfigLoader
{
    private static string CONFIG_PATH = "res://config/gameConfig.json";
    public static GameConfig Load()
    {
        var json = FileAccess.GetFileAsString(CONFIG_PATH);
        
        return JsonSerializer.Deserialize<GameConfig>(json);
    }
}