using LDJam55.Game.Services;

namespace LDJam55.Game.Models;

public delegate Boolean Condition(GameManager gameManager);

public interface IsTag { String Id { get; } Condition? Condition { get; } };
public interface IsPropTag : IsTag { String[] Tags { get; } }
public interface IsLayeredTag : IsTag { String Layer { get; } }

public record Tag(String Id, Condition? Condition = null) : IsTag;
public record PropTag(String Id, String[] Tags, Condition? Condition = null) : Tag(Id, Condition), IsPropTag;
public record LayerTag(String Id, String Layer, Condition? Condition = null) : Tag(Id, Condition), IsLayeredTag;
public record PropLayerTag(String Id, String Layer, String[] Tags, Condition? Condition = null) : LayerTag(Id, Layer, Condition), IsLayeredTag, IsPropTag;

public record DisableTag(Condition Condition) : Tag("disable", Condition);

public interface Frame { String Id { get; } }
public record ContentFrame(String Id, Tag[] Tags, FrameLine[] Lines, String? Title = null) : Frame;
public record Branch(String Id, Boolean Revistable, Func<GameManager, String?> Determinator) : Frame, Invisible;

public interface Invisible;

public interface FrameLine {
    public String Id { get; }
    public Boolean Revistable { get; }
    public Tag[] Tags { get; }
}

public record Story(String Id, Boolean Revistable, Tag[] Tags, String Actor, String Text) : FrameLine;

public record Choice(String Id, Boolean Revistable, String? Actor, String Text, ChoiceOption[] Options, Tag[] Tags) : FrameLine;
public record ChoiceOption(String Text, Action<Session, World>? Action);

public record Checkpoint(String Id, Boolean Revistable, String Type, Task[] Tasks, Tag[] Tags) : FrameLine;
public record Battle(String Id, Boolean Revistable, String Type, Enemy[] Enemies, Equipment[] Rewards, Tag[] Tags) : FrameLine;
public record GameEnd(String Id, Boolean Revistable, Tag[] Tags) : FrameLine, Invisible;
