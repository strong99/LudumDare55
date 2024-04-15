using LDJam55.Game.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LDJam55.Game.Pages;

public partial class Start {
    [Inject] public required ILogger<Start> Logger { get; set; }
    [Inject] public required SessionRepository SessionManager { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IJSRuntime JS { get; set; }

    public Boolean CanContinue { get; private set; }
    public Boolean CanSave { get => SessionManager.Loaded; }

    public IReadOnlyList<String> Bussy { get => _bussy; }
    private List<String> _bussy = [];

    protected override void OnAfterRender(Boolean firstRender) {
        _ = JS.InvokeVoidAsync("playMusic", "music001");
        base.OnAfterRender(firstRender);
    }

    protected override async Task OnParametersSetAsync() {
        CanContinue = await SessionManager.HasSave();

        await base.OnParametersSetAsync();
    }

    public Boolean IsBussy() => _bussy.Count > 0;
    public Boolean IsBussy(String key) => _bussy.Contains(key);

    protected void StartNewGame() {
        SessionManager.New();
        NavigationManager.NavigateTo("play");
    }

    protected async Task Save() {
        _bussy.Add(nameof(Save));
        StateHasChanged();

        await Task.Delay(2000);
        
        try {
            await SessionManager.Save();
        }
        catch (Exception ex) {
            Logger.LogCritical(ex, "Failed to create save");
        }

        _bussy.Remove(nameof(Save));
        StateHasChanged();
    }
}
