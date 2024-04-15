namespace LDJam55.Game.Models;

public class Ally : WithAttribute {
    public String Id { get; }
    public Attribute[] Attributes { get; }
    public Dictionary<String, Equipment?> Equipment { get; }
    public Ally(String id, Attribute[] attributes, Dictionary<String, Equipment?> equipment) {
        Id = id;
        Attributes = attributes.ToArray();
        Equipment = equipment.ToDictionary();
    }

    public Dictionary<String, Single> CalculateAttributes() {
        return Attributes.ToDictionary(a =>
            a.Type,
            a => a.Value * Equipment.Values.Where(p=>p is not null).Sum(p =>
                p.Attributes.Where(a =>
                    a.Type == a.Type
                ).Sum(a =>
                    a.Value
                )
            )
        );
    }
}
