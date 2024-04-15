using LDJam55.Game.Models;
using LDJam55.Game.Pages.Frames;
using LDJam55.Game.Pages.Frames.Widgets;

namespace LDJam55.Game.Services;

public class CountdownTimeWidgetModel : TimeWidgetModel {
    private readonly CheckpointFrameAggegrate _frame;
    public Double Duration { get => _frame.ActiveTask.DurationMs - _frame.ActiveTaskTimeLeftMs; }

    public Double MaxDuration { get => _frame.ActiveTask.DurationMs; }

    public event Action? OnTick;


    public CountdownTimeWidgetModel(CheckpointFrameAggegrate frame) {
        _frame = frame;
    }

    public void Update() {
        OnTick?.Invoke();
    }
}

public class CheckpointFrameAggegrate : ContentFrameLineAggegrate {
    private readonly GameManager _gameManager;
    public String Id { get => FrameLine.Id; }
    public String Path { get => $"{CFrame.Id}:{FrameLine.Id}"; }
    public Models.Task[] Tasks { get => CheckpointFrame.Tasks.Where(t=>!(t is Recruit r && _gameManager.Session.Allies.Any(a=>a.Id == r.Ally.Id))).ToArray(); }
    public Double Duration { get => _gameManager.Session.Duration; }
    public ContentFrame CFrame { get; }
    public FrameLine FrameLine { get => CheckpointFrame; }
    public Checkpoint CheckpointFrame { get; }

    public CountdownTimeWidgetModel? Countdown { get => _countdownModel; }
    private CountdownTimeWidgetModel? _countdownModel;

    public ProgressionWidgetModel ProgressionWidget { get; }
    public BasicTimeWidgetModel TimeWidgetModel { get; }

    public Double ActiveTaskTimeLeftMs { get; private set; } = 0;
    public Models.Task? ActiveTask { get => _activeTask; }
    private Models.Task? _activeTask;

    public Action<SpecialEvent>? OnTaskCompleted;
    public Action<CountdownTimeWidgetModel>? OnTaskStarted;

    public CheckpointFrameAggegrate(GameManager gameManager, ContentFrame frame, Checkpoint line) {
        _gameManager = gameManager;
        CFrame = frame;
        CheckpointFrame = line;

        ProgressionWidget = new(gameManager, frame.Id);
        TimeWidgetModel = new BasicTimeWidgetModel(gameManager);
    }

    private readonly Random _random = new Random();
    public void Update(Double delta) {
        if (_activeTask is not null) {
            ActiveTaskTimeLeftMs -= delta;
            if (ActiveTaskTimeLeftMs < 0) {
                if (_activeTask.Chance > _random.NextDouble()) {
                    if (_activeTask is Training training) {
                        var alliesWithAttributes = _gameManager.Session.Allies.Where(a => a.Attributes.Any(x => x.Type == training.Type)).ToArray();
                        if (alliesWithAttributes.Length > 0) {
                            var randomAlly = alliesWithAttributes[_random.Next(alliesWithAttributes.Length)];
                            var attr = randomAlly.Attributes.First(x => x.Type == training.Type);
                            var idx = Array.IndexOf(randomAlly.Attributes, attr);
                            randomAlly.Attributes[idx] = attr with { Value = attr.Value * training.Multiplier };

                            _activeTask = null;
                            _countdownModel = null;
                            OnTaskCompleted?.Invoke(new TrainingSpecialEvent(DateTime.Now, true, training, randomAlly, attr));
                        }
                        else {
                            _activeTask = null;
                            _countdownModel = null;
                            OnTaskCompleted?.Invoke(new TrainingSpecialEvent(DateTime.Now, false, training, null, null));
                        }
                    }
                    else if (_activeTask is Recruit recruit) {
                        var ally = recruit.Ally;
                        var newAlly = new Ally(
                            ally.Id,
                            [.. ally.Attributes.Select(a => a with { })],
                            ally.Equipment.ToDictionary()
                        );
                        _gameManager.Session.Allies.Add(newAlly);

                        _activeTask = null;
                        _countdownModel = null;
                        OnTaskCompleted?.Invoke(new RecruitedSpecialEvent(DateTime.Now, true, recruit));
                    }
                    else {
                        throw new NotSupportedException("Checkpoint task is not yet supported");
                    }
                }
                else if (_activeTask is Recruit recruit2) {
                    _activeTask = null;
                    _countdownModel = null;
                    OnTaskCompleted?.Invoke(new RecruitedSpecialEvent(DateTime.Now, false, recruit2));
                }
                else if (_activeTask is Training training2) {
                    _activeTask = null;
                    _countdownModel = null;
                    OnTaskCompleted?.Invoke(new TrainingSpecialEvent(DateTime.Now, false, training2, null, null));
                }

            }
            _countdownModel?.Update();
        }
        TimeWidgetModel?.Update();
    }

    public Boolean CanDoNext { get => _activeTask is null; }

    public void GoTo(String id) {
        if (!CanDoNext) {
            return;
        }
        _gameManager.GoTo(id);
    }

    public void GoToNext() {
        if (!CanDoNext) {
            return;
        }
        _gameManager.GoToNext();
    }

    public void Do(Models.Task task) {
        if (!CanDoNext) {
            return;
        }
        _activeTask = task;
        ActiveTaskTimeLeftMs = task.DurationMs;
        _countdownModel = new CountdownTimeWidgetModel(this);
        OnTaskStarted?.Invoke(_countdownModel);
    }
}
