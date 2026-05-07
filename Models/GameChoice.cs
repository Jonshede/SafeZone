namespace SafeZone.Models
{
    public class GameChoice
    {
        public string ButtonText { get; set; } = string.Empty;      // Det användaren ser på knappen
        public string NextNodeId { get; set; } = string.Empty; // Vart man hamnar
        public string? SummaryText { get; set; }              // För ChoiceHistory
        public string? RequiredFlag { get; set; }             // Valfritt: Krav för att se valet
        public string? SetFlag { get; set; }                  // Valfritt: Konsekvens-flagga

        // Optional: name of a level (filename without extension) to load on this choice
        public string? NextLevel { get; set; }
    }
}