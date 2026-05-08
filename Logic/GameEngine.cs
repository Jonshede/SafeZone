using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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

        // Läser bilden direkt från State för att alltid vara i synk (även vid checkpoints)
        public string CurrentBackground => _state.ActiveBackground;

        public GameEngine(HttpClient http, GameStateService state)
        {
            _http = http;
            _state = state;
        }

        public async Task LoadLevel(string levelName)
        {
            IsLoading = true;
            var url = $"game-data/{levelName}.json";

            try
            {
                var data = await _http.GetFromJsonAsync<GameLevel>(url);
                if (data != null)
                {
                    _loadedLevel = data;
                    _activeScenarios = data.Scenarios.ToDictionary(s => s.Id);

                    _state.CurrentLevelName = levelName;
                    _state.SetCurrentNode(data.StartNodeId);

                    // Sätt bild från startnoden eller standard
                    if (!string.IsNullOrEmpty(CurrentScenario?.ImagePath))
                        _state.ActiveBackground = CurrentScenario.ImagePath;
                    else
                        _state.ActiveBackground = "images/start-bg.jpg";

                    if (CurrentScenario?.IsCheckpoint == true)
                        _state.SaveCheckpoint();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameEngine] Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SelectChoice(GameChoice gameChoice)
        {
            _state.AddChoiceSummary(gameChoice.SummaryText);

            if (!string.IsNullOrEmpty(gameChoice.NextLevel))
            {
                await LoadLevel(gameChoice.NextLevel);
                if (!string.IsNullOrEmpty(gameChoice.NextNodeId))
                    _state.SetCurrentNode(gameChoice.NextNodeId);
            }
            else
            {
                _state.SetCurrentNode(gameChoice.NextNodeId);
            }

            // Uppdatera bild om det finns en ny i nästa scen
            if (!string.IsNullOrEmpty(CurrentScenario?.ImagePath))
            {
                _state.ActiveBackground = CurrentScenario.ImagePath;
            }

            if (CurrentScenario?.IsCheckpoint == true)
            {
                _state.SaveCheckpoint();
            }
        }

        public void ResetEngine()
        {
            _state.ActiveBackground = "images/start-bg.jpg";
            _activeScenarios.Clear();
            _loadedLevel = null;
            IsLoading = true;
        }
    }
}