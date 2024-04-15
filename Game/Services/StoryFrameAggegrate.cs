using LDJam55.Game.Models;

namespace LDJam55.Game.Services;

public class StoryFrameAggegrate : ContentFrameLineAggegrate {
    private readonly GameManager _gameManager;
    public String Id { get => FrameLine.Id; }
    public String Path { get => $"{CFrame.Id}:{FrameLine.Id}"; }
    public String? Actor { get => StoryFrame?.Actor; }
    public String Text { get => StoryFrame.Text; }
    public Double Duration { get => _gameManager.Session.Duration; }
    public ContentFrame CFrame { get; }
    public FrameLine FrameLine { get => StoryFrame; }
    public Story StoryFrame { get; }

    public StoryFrameAggegrate(GameManager gameManager, ContentFrame frame, Story story) {
        _gameManager = gameManager;
        CFrame = frame;
        StoryFrame = story;
    }

    public void Update(Double delta) {

    }

    public void GoToNext() {
        _gameManager.GoToNext();
    }
}
