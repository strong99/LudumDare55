using LDJam55.Game.Models;
using LDJam55.Game.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Task = System.Threading.Tasks.Task;

namespace LDJam55.Game.Pages;

public partial class Preview {
    [Inject] public required SettingsRepository SettingsRepository { get; set; }
    [Inject] public required SessionRepository SessionRepository { get; set; }
    [Inject] public required WorldRepository WorldRepository { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IJSRuntime JS { get; set; }

    [Parameter][SupplyParameterFromQuery] public String? ActiveFrameId { get; set; }

    public IReadOnlyList<OuterFrame> OuterFrames { get => _outerFrames; }
    private List<OuterFrame> _outerFrames = new();

    public GameManager.TempFrameLine[] Frames { get => _gameManager?.GetFrameLines(true) ?? []; }

    private GameManager? _gameManager;
    private Int32 _margin = 5;

    protected override Task OnParametersSetAsync() {

        var world = WorldRepository.World;
        var session = SessionRepository.Session;

        session.CurrentFrameId = ActiveFrameId ??= world.Frames.First().Id;

        if (_gameManager?.Session != session && _gameManager?.World != world) {
            _gameManager = new GameManager(world, session);
        }

        _outerFrames = _gameManager.GetNeighbourFrames(_margin);


        return base.OnParametersSetAsync();
    }

    public void Toggle(String id) {
        if (!WorldRepository.World.Frames.Any(p => p is ContentFrame cf && cf.Lines.Any(l => $"{p.Id}:{l.Id}" == id))) {
            throw new Exception("Frame not found");
        }

        if (_gameManager is null) {
            throw new Exception("Game manager not loaded");
        }

        if (!_gameManager.Session.FrameLineIds.Contains(id)) {
            _gameManager.Session.FrameLineIds.Add(id);
        }
        else {
            _gameManager.Session.FrameLineIds.Remove(id);
        }
    }

    private Boolean _isBussy = false;
    public async ValueTask SelectFrame(String id) {
        if (_isBussy || _gameManager is null) {
            return;
        }

        _isBussy = true;

        var frameLines = _gameManager.GetFrameLines();

        if (!frameLines.Any(p => p.IsMatch(id))) {
            throw new Exception("Frame not found");
        }

        if (_gameManager is null) {
            throw new Exception("Game manager not loaded");
        }

        if (!_gameManager.Session.FrameLineIds.Contains(id)) {
            _gameManager.Session.FrameLineIds.Add(id);
        }

        var previousFrameId = _gameManager.Session.CurrentFrameId;
        var nextFrameId = id;

        _outerFrames = _gameManager.GetNeighbourFrames(_margin, previousFrameId, nextFrameId);
        StateHasChanged();

        await Task.Delay(1);

        ActiveFrameId = _gameManager.Session.CurrentFrameId = id;

        _gameManager = new GameManager(WorldRepository.World, SessionRepository.Session);
        _outerFrames = _gameManager.GetNeighbourFrames(_margin, nextFrameId, previousFrameId);
        await InvokeAsync(StateHasChanged);

        await Task.Delay(GameManager.FrameChangeAnimationTime);
        _outerFrames = _gameManager.GetNeighbourFrames(_margin);

        _isBussy = false;
        await InvokeAsync(StateHasChanged);
    }
}
