namespace LDJam55.Game.Models;

public class Session {
    public String? CurrentFrameId { get; set; }
    public List<String> Tags { get; set; } = new();
    public List<String> FrameLineIds { get; set; } = new();
    public List<Equipment> Equipment { get; set; } = new();
    public Double Timestamp { get; set; }
    public List<Ally> Allies { get; set; } = new();
    public Double Duration { get; set; }
}

public record Equipment(String Id, String Type, String Variant, Attribute[] Attributes);
public record Attribute(String Type, Single Value);
