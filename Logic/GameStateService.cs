using SafeZone.Models;

public class GameStateService
{
    // --- Aktiv speldata (det som ändras hela tiden) ---
    public string CurrentNodeId { get; set; } = "intro";
    public List<string> ChoiceHistory { get; private set; } = new List<string>();

    // --- Checkpoint-data (det som sparats) ---
    private string _checkpointNodeId = "Start";
    private List<string> _checkpointHistory = new List<string>();

    public event Action OnChange;

    public void HandleChoice(GameChoice gameChoice)
    {
        if (!string.IsNullOrEmpty(gameChoice.SummaryText))
            ChoiceHistory.Add(gameChoice.SummaryText);

        CurrentNodeId = gameChoice.NextNodeId;
        NotifyStateChanged();
    }

    // Kallar på denna när en nivå börjar eller vid en säker plats
    public void SaveCheckpoint()
    {
        _checkpointNodeId = CurrentNodeId;

        // Vi skapar en helt NY lista (en kopia) av historiken.
        // Om vi bara skriver _checkpointHistory = ChoiceHistory 
        // så kommer båda peka på samma lista, vilket vi inte vill.
        _checkpointHistory = new List<string>(ChoiceHistory);

        NotifyStateChanged();
    }

    // Kallar på denna om spelaren dör
    public void ResetToCheckpoint()
    {
        CurrentNodeId = _checkpointNodeId;

        // Återställ historiken till hur den såg ut vid sparögonblicket
        ChoiceHistory = new List<string>(_checkpointHistory);

        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}