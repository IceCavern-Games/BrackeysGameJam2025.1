using System.Collections.Generic;

/// <summary>
/// Mappings of Action Name to Text to Display for Button Prompts.
/// </summary>
public static class PromptMappings
{
    public static Dictionary<string, string> Gameplay = new() {
        {"Move", "Move"},
        {"Paint", "Paint"},
        {"Erase", "Erase"}
    };
    public static Dictionary<string, string> Dialogue = new() {
        {"Submit", "Confirm"}
    };
    public static Dictionary<string, string> PauseMenu = new() {
        {"Navigate", "Navigate"},
        {"Submit", "Select"},
        {"Cancel", "Cancel"}
    };
    public static Dictionary<string, string> OptionsMenu = new() {
        {"Navigate", "Navigate"},
        {"Submit", "Select"},
        {"Cancel", "Back"},
    };
    public static Dictionary<string, string> TitleMenu = new() {
        {"Navigate", "Navigate"},
        {"Submit", "Select"},
    };
}
