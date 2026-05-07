namespace SafeZone.Models
{
    public class GameLevel
    {
            public string LevelName { get; set; } = string.Empty;
            public string StartNodeId { get; set; } = "Start";
            public List<GameScenario> Scenarios { get; set; } = new();
    }
}
