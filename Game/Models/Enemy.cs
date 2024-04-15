namespace LDJam55.Game.Models;

public class Enemy : WithAttribute {
    public String Name { get; }
    public String Type { get; }
    public String Variant { get; }
    public Attribute[] Attributes { get; }
    public Enemy(String name, String type, String variant, Attribute[] attributes) {
        Name = name;
        Type = type;
        Variant = variant;
        Attributes = attributes.ToArray();
    }
    public Dictionary<String, Single> CalculateAttributes() {
        return Attributes.ToDictionary(attribute => attribute.Type, attribute => attribute.Value);
    }
}
