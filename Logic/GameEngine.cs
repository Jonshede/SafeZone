using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using SafeZone.Models;
using System.Linq;

namespace SafeZone.Logic
{
    public class GameEngine
    {
        private readonly HttpClient _http;
        private readonly GameStateService _state;
        private Dictionary<string, GameScenario> _activeScenarios = new();

        private GameLevel? _loadedLevel;
        public GameLevel? LoadedLevel => _loadedLevel;

        public GameScenario? CurrentScenario => _activeScenarios.GetValueOrDefault(_state.CurrentNodeId);
        public bool IsLoading { get; private set; } = true;

        public GameEngine(HttpClient http, GameStateService state)
        {
            _http = http;
            _state = state;
        }

        public async Task LoadLevel(string levelName)
        {
            IsLoading = true;
            var data = await _http.GetFromJsonAsync<GameLevel>($"game-data/{levelName}.json");

            if (data != null)
            {
                _loadedLevel = data;
                _activeScenarios = data.Scenarios.ToDictionary(s => s.Id);

                // keep state in sync with loaded level
                _state.CurrentLevelName = levelName;
                _state.SetCurrentNode(data.StartNodeId);
            }
            IsLoading = false;
        }

        // Now async so we can load another level as part of a choice
        public async Task SelectChoice(GameChoice gameChoice)
        {
            // Record summary text
            if (!string.IsNullOrEmpty(gameChoice.SummaryText))
                _state.ChoiceHistory.Add(gameChoice.SummaryText);

            // If the choice specifies a level, load it
            if (!string.IsNullOrEmpty(gameChoice.NextLevel))
            {
                await LoadLevel(gameChoice.NextLevel);

                // If a specific node is provided use it, otherwise keep the loaded level StartNodeId
                if (!string.IsNullOrEmpty(gameChoice.NextNodeId))
                    _state.SetCurrentNode(gameChoice.NextNodeId);
                else
                    _state.SetCurrentNode(_state.CurrentNodeId); // triggers notification for start node
            }
            else
            {
                // Same-level navigation
                _state.SetCurrentNode(gameChoice.NextNodeId);
            }

            // If the new scenario is a checkpoint, save it
            if (CurrentScenario?.IsCheckpoint == true)
            {
                _state.SaveCheckpoint();
            }
        }
    }
}
