namespace LDJam55.Game.Models;

public interface WithAttribute {
    Attribute[] Attributes { get; }
    Dictionary<String, Single> CalculateAttributes();
}
