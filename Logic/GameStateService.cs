using SafeZone.Models;

namespace SafeZone.Logic
{
    public class GameStateService
    {
        // --- Aktiv speldata (det som ändras hela tiden) ---
        public string CurrentLevelName { get; set; } = "level1";
        public string CurrentNodeId { get; set; } = "intro";
        public List<string> ChoiceHistory { get; private set; } = new();

        // --- Checkpoint-data (det som sparats) ---
        private string _checkpointLevelName = "level1";
        private string _checkpointNodeId = "Start";
        private List<string> _checkpointHistory = new();

        public event Action? OnChange;

        public void HandleChoice(GameChoice gameChoice)
        {
            if (!string.IsNullOrEmpty(gameChoice.SummaryText))
                ChoiceHistory.Add(gameChoice.SummaryText);

            CurrentNodeId = gameChoice.NextNodeId;
            NotifyStateChanged();
        }

        // Public helper to set the current node and notify subscribers
        public void SetCurrentNode(string nodeId)
        {
            CurrentNodeId = nodeId;
            NotifyStateChanged();
        }

        // Kallar på denna när en nivå börjar eller vid en säker plats
        public void SaveCheckpoint()
        {
            _checkpointLevelName = CurrentLevelName;
            _checkpointNodeId = CurrentNodeId;
            _checkpointHistory = new List<string>(ChoiceHistory);
            NotifyStateChanged();
        }

        // Kallar på denna om spelaren dör eller när användaren återupptar från checkpoint
        public void ResetToCheckpoint()
        {
            CurrentLevelName = _checkpointLevelName;
            CurrentNodeId = _checkpointNodeId;
            ChoiceHistory = new List<string>(_checkpointHistory);
            NotifyStateChanged();
        }

        // Clears entire progress and history (use for restart)
        public void ClearAll()
        {
            CurrentLevelName = "level1";
            CurrentNodeId = "intro";
            ChoiceHistory.Clear();
            _checkpointLevelName = "level1";
            _checkpointNodeId = "Start";
            _checkpointHistory.Clear();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}