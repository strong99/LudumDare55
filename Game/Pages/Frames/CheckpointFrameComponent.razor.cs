using LDJam55.Game.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LDJam55.Game.Pages.Frames;

public partial class CheckpointFrameComponent : IDisposable {
    [Parameter] public required CheckpointFrameAggegrate Frame {
        get => _frame;
        set {
            var oldValue = _frame;
            var newValue = _frame = value;

            if (oldValue != newValue) {
                if (oldValue is not null) {
                    oldValue.OnTaskCompleted -= Frame_OnTaskCompleted;
                    oldValue.OnTaskStarted -= Frame_OnTaskStarted;
                }

                _specialEvents.Clear();

                if (newValue is not null) {
                    Frame.OnTaskCompleted += Frame_OnTaskCompleted;
                    Frame.OnTaskStarted += Frame_OnTaskStarted;
                }
            }
        }
    }
    private CheckpointFrameAggegrate _frame = default!;

    [Parameter] public required Action ShowEquipment { get; set; }

    public IReadOnlyList<SpecialEvent> SpecialEvents { get => _specialEvents; }
    private List<SpecialEvent> _specialEvents { get; } = new();

    [Inject] public required IJSRuntime JS { get; set; }

    public String FrameTypeName { get => Frame.GetType().Name.Replace(nameof(FrameAggegrate), ""); }
    protected override void OnAfterRender(Boolean firstRender) {
        JS.InvokeVoidAsync("playMusic", $"music005town");
        base.OnAfterRender(firstRender);
    }

    private void Frame_OnTaskCompleted(SpecialEvent specialEvent) {
        var now = DateTime.Now;
        _specialEvents.RemoveAll(e=> (now - e.When).TotalSeconds > 10);
        _specialEvents.Add(specialEvent);
        InvokeAsync(StateHasChanged);
    }
    private void Frame_OnTaskStarted(CountdownTimeWidgetModel countDown) {
        InvokeAsync(StateHasChanged);
    }

    protected void Next() {
        Frame.GoToNext();
    }

    protected void Do(Models.Task task) {
        Frame.Do(task);
    }

    public void GoTo(String id) {
        Frame.GoTo(id);
    }

    public void Dispose() {
        Frame.OnTaskCompleted -= Frame_OnTaskCompleted;
        Frame.OnTaskStarted -= Frame_OnTaskStarted;
    }
}
