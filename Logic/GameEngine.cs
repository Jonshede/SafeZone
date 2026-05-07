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
            // Hämtar JSON från wwwroot/data/level1.json
            var data = await _http.GetFromJsonAsync<GameLevel>($"data/{levelName}.json");

            if (data != null)
            {
                // Gör om listan till en Dictionary för att snabbt hitta scenarion via ID
                _activeScenarios = data.Scenarios.ToDictionary(s => s.Id);
                _state.CurrentNodeId = data.StartNodeId;
            }
            IsLoading = false;
        }

        public void SelectChoice(GameChoice gameChoice)
        {
            // 1. Hantera flaggor här om ni har sådana
            // 2. Uppdatera statet via din service
            _state.HandleChoice(gameChoice);

            // 3. Kolla om det nya scenariot är en checkpoint
            if (CurrentScenario?.IsCheckpoint == true)
            {
                _state.SaveCheckpoint();
            }
        }
    }
}
