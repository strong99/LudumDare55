using Microsoft.AspNetCore.Components;

namespace LDJam55.Game.Pages.Frames.Widgets;

public interface TimeWidgetModel {
    Double Duration { get; }
    Double MaxDuration { get; }
    event Action? OnTick;
}

public partial class TimeWidget {

    [Parameter]
    public required TimeWidgetModel Model {
        get => _model;
        set {
            var oldFrame = _model;
            var newFrame = _model = value;

            if (oldFrame != newFrame) {
                if (oldFrame is not null) {
                    oldFrame.OnTick -= OnTick;
                }
                if (newFrame is not null) {
                    newFrame.OnTick += OnTick;
                }
            }
        }
    }
    private TimeWidgetModel _model = default!;

    [Parameter] public required String? Summary { get; set; }

    protected override void OnParametersSet() {
        base.OnParametersSet();
    }

    public void OnTick() {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose() {
        if (Model is not null) {
            Model.OnTick -= OnTick;
        }
    }
}
