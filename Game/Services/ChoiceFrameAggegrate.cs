using LDJam55.Game.Models;

namespace LDJam55.Game.Services;

public record ChoiceFrameOption(String Text) { 
    public String Id { get => Text; } 
}

public class ChoiceFrameAggegrate : ContentFrameLineAggegrate {
    private readonly GameManager _gameManager;
    public String Id { get => FrameLine.Id; }
    public String Path { get => $"{CFrame.Id}:{FrameLine.Id}"; }
    public Double Duration { get => _gameManager.Session.Duration; }
    public String? Actor { get => ChoiceFrame.Actor; }
    public String Text { get => ChoiceFrame.Text; }
    public ContentFrame CFrame { get; }
    public FrameLine FrameLine { get => ChoiceFrame; }
    public Choice ChoiceFrame { get; }

    public ChoiceFrameOption[] Options { get; }

    public ChoiceFrameAggegrate(GameManager gameManager, ContentFrame frame, Choice line) {
        _gameManager = gameManager;
        CFrame = frame;
        ChoiceFrame = line;
        Options = line.Options.Select(o=>new ChoiceFrameOption(o.Text)).ToArray();
    }

    public void SelectChoice(String choiceId) {
        var session = _gameManager.Session;
        var world = _gameManager.World;

        var option = ChoiceFrame.Options.Single(p => p.Text == choiceId);
        option.Action?.Invoke(session, world);

        _gameManager.GoToNext();
    }

    public void Update(Double delta) {

    }
}
