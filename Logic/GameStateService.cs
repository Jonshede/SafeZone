using System;
using System.Collections.Generic;
using SafeZone.Models;

namespace SafeZone.Logic
{
    public class GameStateService
    {
        // --- Aktiv speldata ---
        public string CurrentLevelName { get; set; } = "level1";
        public string CurrentNodeId { get; set; } = "intro";

        // Denna håller koll på bilden som visas just nu
        public string ActiveBackground { get; set; } = "images/start-bg.jpg";

        public List<GameSummaryEntry> ChoiceHistory { get; private set; } = new();

        // --- Checkpoint-data ---
        private string _checkpointLevelName = "level1";
        private string _checkpointNodeId = "intro";
        private string _checkpointBackground = "images/start-bg.jpg";
        private List<GameSummaryEntry> _checkpointHistory = new();

        public event Action? OnChange;

        // Lägger till text i sammanfattningen
        public void AddChoiceSummary(string? summaryText)
        {
            if (string.IsNullOrEmpty(summaryText))
                return;

            ChoiceHistory.Add(new GameSummaryEntry(CurrentLevelName ?? string.Empty, summaryText));
            NotifyStateChanged();
        }

        // Uppdaterar vilken nod vi är på
        public void SetCurrentNode(string nodeId)
        {
            CurrentNodeId = nodeId;
            NotifyStateChanged();
        }

        public void SaveCheckpoint()
        {
            _checkpointLevelName = CurrentLevelName;
            _checkpointNodeId = CurrentNodeId;
            _checkpointBackground = ActiveBackground; // Sparar bilden!
            _checkpointHistory = new List<GameSummaryEntry>(ChoiceHistory);
            NotifyStateChanged();
        }

        public void ResetToCheckpoint()
        {
            CurrentLevelName = _checkpointLevelName;
            CurrentNodeId = _checkpointNodeId;
            ActiveBackground = _checkpointBackground; // Återställer bilden!
            ChoiceHistory = new List<GameSummaryEntry>(_checkpointHistory);
            NotifyStateChanged();
        }

        public void ClearAll()
        {
            CurrentLevelName = "level1";
            CurrentNodeId = "intro";
            ActiveBackground = "images/start-bg.jpg";
            ChoiceHistory.Clear();
            _checkpointLevelName = "level1";
            _checkpointNodeId = "intro";
            _checkpointBackground = "images/start-bg.jpg";
            _checkpointHistory.Clear();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}