using System;
using System.Net.Http;
using System;
using System.Net.Http;
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
            var url = $"game-data/{levelName}.json";
            Console.WriteLine($"[GameEngine] Attempting to load level URL: {url}");

            try
            {
                var data = await _http.GetFromJsonAsync<GameLevel>(url);
                if (data == null)
                {
                    Console.WriteLine($"[GameEngine] LoadLevel returned null for {url}");
                }
                else
                {
                    _loadedLevel = data;
                    _activeScenarios = data.Scenarios.ToDictionary(s => s.Id);

                    // keep state in sync with loaded level
                    _state.CurrentLevelName = levelName;
                    _state.SetCurrentNode(data.StartNodeId);

                    // If the start node is a checkpoint, save it so ResetToCheckpoint restores here
                    if (CurrentScenario?.IsCheckpoint == true)
                    {
                        _state.SaveCheckpoint();
                        Console.WriteLine($"[GameEngine] Saved checkpoint for level '{levelName}' node '{data.StartNodeId}'");
                    }

                    Console.WriteLine($"[GameEngine] Loaded level '{levelName}' with start node '{data.StartNodeId}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameEngine] Exception loading {url}: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Now async so we can load another level as part of a choice
        public async Task SelectChoice(GameChoice gameChoice)
        {
            // Record summary text with level
            if (!string.IsNullOrEmpty(gameChoice.SummaryText))
                _state.AddChoiceSummary(gameChoice.SummaryText);

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
