using LDJam55.Game.Models;

namespace LDJam55.Game.Services;

public class ContentFrameAggegrate : FrameAggegrate {
    public String Id { get; }
    public String Path { get; }
    public IReadOnlyList<Tag> Tags { get; }
    public IReadOnlyList<IsPropTag> Props { get; }

    public ContentFrameLineAggegrate? Line { get; }

    public ContentFrameAggegrate(GameManager gameManager, ContentFrame frame, FrameLine? line) {
        Id = frame.Id;
        Path = line is null ? Id : $"{frame.Id}:{line.Id}";

        Tags = frame.Tags.Where(t => t is not IsPropTag && t.Condition?.Invoke(gameManager) != false)
            .Union(line is null ?[] : line.Tags.Where(t => t is not IsPropTag && t.Condition?.Invoke(gameManager) != false))
            .ToArray();
        Props = frame.Tags.OfType<IsPropTag>().Where(t => t.Condition?.Invoke(gameManager) != false)
            .Union(line is null ? [] : line.Tags.OfType<IsPropTag>().Where(t => t.Condition?.Invoke(gameManager) != false))
            .ToArray();

        Line = line switch {
            Battle battle => new BattleFrameAggegrate(gameManager, frame, battle),
            Story story => new StoryFrameAggegrate(gameManager, frame, story),
            Choice choice => new ChoiceFrameAggegrate(gameManager, frame, choice),
            Checkpoint checkpoint => new CheckpointFrameAggegrate(gameManager, frame, checkpoint),
            _ => null
        };
    }

    public void Update(Double delta) {
        Line?.Update(delta);
    }
}
