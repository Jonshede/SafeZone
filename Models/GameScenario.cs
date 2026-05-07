namespace SafeZone.Models
{
    public class GameScenario
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;      // Själva berättelsen
        public string? ImagePath { get; set; }                // För framtida pixelart
        public List<GameChoice> Choices { get; set; } = new();
        public bool IsCheckpoint { get; set; }
        // New: mark scenarios that end the level / should jump to summary
        public bool IsTerminal { get; set; }
    }
}
