using LDJam55.Game.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LDJam55.Game.Pages.Frames;

public partial class StoryFrameComponent {
    [Parameter] public required StoryFrameAggegrate Frame { get; set; }

    [Inject] public required IJSRuntime JS { get; set; }

    protected override void OnAfterRender(Boolean firstRender) {
        JS.InvokeVoidAsync("playMusic", $"music002");
        base.OnAfterRender(firstRender);
    }

    protected void Next() {
        Frame.GoToNext();
    }

    protected override Task OnInitializedAsync() {
        return base.OnInitializedAsync();
    }
}
