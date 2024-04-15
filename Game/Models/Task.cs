namespace LDJam55.Game.Models;

public interface Task { 
    Int32 DurationMs { get; } 
    Single Chance { get; } 
}

public record Training(Int32 DurationMs, String Type, Single Chance, Single Multiplier) : Task;
public record Recruit(Int32 DurationMs, Single Chance, Ally Ally) : Task;
