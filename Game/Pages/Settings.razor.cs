using LDJam55.Game.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LDJam55.Game.Pages;

public partial class Settings {
    [Inject] public required SettingsRepository SettingsManager { get; set; }
    [Inject] public required IJSRuntime JS { get; set; }

    public Int32 MusicVolume { get => (Int32)(SettingsManager.MusicVolume * 100); set { var v = value / 100.0f;  SettingsManager.MusicVolume = v; JS.InvokeVoidAsync("onMusicVolumeUpdated", v); } }
    public Int32 OtherVolume { get => (Int32)(SettingsManager.OtherVolume * 100); set { var v = value / 100.0f;  SettingsManager.OtherVolume = v; JS.InvokeVoidAsync("onOtherVolumeUpdated", v); } }

    protected override void OnAfterRender(Boolean firstRender) {
        _ = JS.InvokeVoidAsync("playMusic", "music001");
        base.OnAfterRender(firstRender);
    }

    protected override async Task OnInitializedAsync() {
        await SettingsManager.Load();
        await base.OnInitializedAsync();
    }
}
