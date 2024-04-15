using LDJam55.Game.Models;
using LDJam55.Game.Pages.Frames.Widgets;
using LDJam55.Game.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Attribute = LDJam55.Game.Models.Attribute;

namespace LDJam55.Game.Pages.Frames;

public record DamageEvent(DateTime When, EnemyAggegrate Source, String Type, Single Damage);
public interface SpecialEvent { 
    public DateTime When { get; }
}
public record EnemySpecialEvent(DateTime When, EnemyAggegrate Source, String Type) : SpecialEvent;
public record EquipmentDropSpecialEvent(DateTime When, Equipment Equipment) : SpecialEvent;
public record RecruitedSpecialEvent(DateTime When, Boolean Success, Recruit Task) : SpecialEvent;
public record TrainingSpecialEvent(DateTime When, Boolean Success, Training Task, Ally? Ally, Attribute? Attribute) : SpecialEvent;

public partial class BattleFrameComponent : IDisposable {
    [Parameter] public required BattleFrameAggegrate Frame { 
        get => _frame; 
        set {
            var oldValue = _frame;
            var newValue = _frame = value;

            if (oldValue != newValue) {
                Frame.OnDamageApplied -= Frame_OnDamageApplied;
                Frame.OnEquipmentDropped -= Frame_OnEquipmentDropped;

                _specialEvents.Clear();

                Frame.OnDamageApplied += Frame_OnDamageApplied;
                Frame.OnEquipmentDropped += Frame_OnEquipmentDropped;
            }
        }
    }
    private BattleFrameAggegrate _frame = default!;

    [Inject] public required IJSRuntime JS { get; set; }

    public IReadOnlyList<DamageEvent> DamageEvents { get => _damageEvents; }
    private List<DamageEvent> _damageEvents { get; } = new();

    public IReadOnlyList<SpecialEvent> SpecialEvents { get => _specialEvents; }
    private List<SpecialEvent> _specialEvents { get; } = new();

    protected override void OnParametersSet() {

        var now = DateTime.Now;
        _damageEvents.RemoveAll(p => (now - p.When).TotalSeconds > 5);

        base.OnParametersSet();
    }

    protected override void OnAfterRender(Boolean firstRender) {
        if (firstRender) {
            JS.InvokeVoidAsync("scrollToElement", $"progressBar_{Frame.Path}");
            JS.InvokeVoidAsync("playMusic", $"music004battle");
        }
        base.OnAfterRender(firstRender);
    }

    private void Frame_OnDamageApplied(EnemyAggegrate arg1, Dictionary<String, Single> arg2) {
        foreach (var arg in arg2) {
            _damageEvents.Add(new(DateTime.Now, arg1, arg.Key, arg.Value));

            if (arg1.IsDead) {
                _specialEvents.Add(new EnemySpecialEvent(DateTime.Now, arg1, "Died"));
            }
        }
        InvokeAsync(StateHasChanged);
    }

    private void Frame_OnEquipmentDropped(Equipment equipment) {
        _specialEvents.Add(new EquipmentDropSpecialEvent(DateTime.Now, equipment));
        InvokeAsync(StateHasChanged);
    }

    public void Next() {
        Frame.GoToNext();
    }

    public void GoTo(String id) {
        Frame.GoTo(id);
    }

    public void Dispose() {
        Frame.OnDamageApplied -= Frame_OnDamageApplied;
        Frame.OnEquipmentDropped -= Frame_OnEquipmentDropped;
    }
}
