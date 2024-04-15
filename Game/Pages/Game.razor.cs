using LDJam55.Game.Models;
using LDJam55.Game.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Task = System.Threading.Tasks.Task;

namespace LDJam55.Game.Pages;

public interface Panel {
}
public class EquipmentPanel : Panel {
    public IReadOnlyList<Ally> Allies { get => _session.Allies; }
    public IReadOnlyList<Equipment> Equipment { get => _session.Equipment; }
    private Session _session;

    public EquipmentPanel(Session session) {
        _session = session;
    }

    public void MoveFromAllyToAlly(Equipment equipment, Ally ally, String fromSlot, String toSlot) {
        if (equipment.Type != toSlot) {
            return;
        }

        var other = ally.Equipment[fromSlot];
        if (other is not null && other.Type != fromSlot) {
            ally.Equipment[toSlot] = ally.Equipment[fromSlot];
            ally.Equipment[fromSlot] = null;
            _session.Equipment.Add(other);
        }
        else {
            (ally.Equipment[fromSlot], ally.Equipment[toSlot]) = (ally.Equipment[toSlot], ally.Equipment[fromSlot]);
        }
    }

    public void MoveFromAllyToInventory(Equipment equipment, Ally ally, String equipmentSource) {
        _session.Equipment.Add(equipment);
        ally.Equipment[equipmentSource] = null;
    }

    public void MoveFromInventoryToAlly(Equipment equipment, Ally ally, String slot) {
        if (equipment.Type != slot) {
            return;
        }

        var originalEquipment = ally.Equipment[slot];
        if (originalEquipment is not null) {
            _session.Equipment.Add(originalEquipment);
        }

        _session.Equipment.Remove(equipment);
        ally.Equipment[slot] = equipment;
    }

    public Boolean CanDrop(Equipment equipment, Ally ally, String slot) {
        if (equipment.Type != slot) {
            return false;
        }
        return true;
    }
}

public partial class Game : IDisposable {
    public static readonly Object UiSectionId = new();

    [Inject] public required SettingsRepository SettingsRepository { get; set; }
    [Inject] public required SessionRepository SessionRepository { get; set; }
    [Inject] public required WorldRepository WorldRepository { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IJSRuntime JS { get; set; }

    public Panel? Panel { get; set; }
    public ContentFrameAggegrate? Frame { get => _gameManager?.Frame switch {
        null => null,
        ContentFrameAggegrate fr => fr,
        _ => throw new NotSupportedException($"Only content frames can be displayed, not {_gameManager?.Frame}")
    }; }
    public ContentFrameLineAggegrate? Line { get => Frame?.Line; }
    public IReadOnlyList<OuterFrame> OuterFrames { get; private set; } = new List<OuterFrame>();

    private GameManager? _gameManager;
    private Int32 _margin = 3;

    protected override async System.Threading.Tasks.Task OnParametersSetAsync() {
        if (!SessionRepository.Loaded
         && await SessionRepository.Load()) {
            // Continueing session
        }

        var world = WorldRepository.World;
        var session = SessionRepository.Session;
        if (_gameManager?.Session != session && _gameManager?.World != world) {
            if (_gameManager is not null) {
                _gameManager.OnFinished -= GameManager_OnFinishedChanged;
                _gameManager.OnFrameChanged -= GameManager_OnFrameChanged;
            }
            _gameManager = new GameManager(world, session);
            _gameManager.OnFrameChanged += GameManager_OnFrameChanged;
            _gameManager.OnFinished += GameManager_OnFinishedChanged;

            OuterFrames = _gameManager.GetNeighbourFrames(_margin);
        }

        await base.OnParametersSetAsync();
    }

    private async void GameManager_OnFrameChanged(GameManager gameManager, String? fromId, String? toId) {
        if (_gameManager is null) {
            return;
        }
        
        OuterFrames = _gameManager.GetNeighbourFrames(_margin, fromId, toId);

        var refreshTask =InvokeAsync(StateHasChanged);
        var delayTask = Task.Delay(GameManager.FrameChangeAnimationTime);

        await Task.WhenAll(
            refreshTask,
            delayTask,
            SessionRepository.SaveToCache().AsTask()
        );

        OuterFrames = _gameManager.GetNeighbourFrames(_margin);
        await InvokeAsync(StateHasChanged);
    }

    private void GameManager_OnFinishedChanged(GameManager gameManager) {
        NavigationManager.NavigateTo("./");
    }

    public void Dispose() {
        if (_gameManager is not null) {
            _gameManager.OnFinished -= GameManager_OnFinishedChanged;
            _gameManager.OnFrameChanged -= GameManager_OnFrameChanged;
            _gameManager.Dispose();
        }
    }

    public void ShowEquipment() {
        if (_gameManager is null) {
            return;
        }

        Panel = new EquipmentPanel(_gameManager.Session);
    }

    public void ClosePanel() {
        Panel = null;
    }
}
