using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SafeZone;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

// Representerar ett val i berättelsen
public class Choice
{
    public string ButtonText { get; set; }     // Det spelaren klickar på
    public string SummaryText { get; set; }    // Texten som visas i sammanfattningen (t.ex. "Du valde att gömma dig")
    public string NextNodeId { get; set; }
    public bool EndsLevel { get; set; }        // Flagga för att visa sammanfattningsskärmen
}

// Håller koll på spelarens resa
public class GameSession
{
    public string CurrentNodeId { get; set; } = "Start";
    public List<string> ChoiceHistory { get; set; } = new List<string>();

    // För checkpoints
    public string LastCheckpointNodeId { get; set; } = "Start";
    public List<string> CheckpointHistory { get; set; } = new List<string>();

    public void SaveCheckpoint()
    {
        LastCheckpointNodeId = CurrentNodeId;
        // Vi skapar en kopia av listan så att historiken fryses vid checkpointen
        CheckpointHistory = new List<string>(ChoiceHistory);
    }

    public void ResetToCheckpoint()
    {
        CurrentNodeId = LastCheckpointNodeId;
        ChoiceHistory = new List<string>(CheckpointHistory);
    }
}