using LDJam55.Game.Models;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace LDJam55.Game.Pages.Panels;

public class EquipmentAllyAggegrate {
    public String Id { get; }
    public IReadOnlyDictionary<String, Single> Attack { get; }
    public IReadOnlyDictionary<String, Equipment?> Equipment { get; }
    public EquipmentAllyAggegrate(String id, Dictionary<String, Single> attack, IReadOnlyDictionary<String, Equipment?> equipment) {
        Id = id;
        Attack = attack;
        Equipment = equipment;
    }
}

public partial class EquipmentComponent {
    [Parameter] public required EquipmentPanel Model { get; set; }
    [Parameter] public required Action ClosePanel { get; set; }
    public EquipmentAllyAggegrate? Ally { get; set; }
    private Dictionary<String, String> _dropClass = new();

    protected override void OnParametersSet() {
        if (Ally is null) {
            Select(Model.Allies.First());
        }

        base.OnParametersSet();
    }

    private void Select(Ally ally) {
        Ally = new EquipmentAllyAggegrate(ally.Id, ally.CalculateAttributes(), ally.Equipment);
    }
    
    private String? _equipmentSource = null;
    private String? _equipmentFilter = null;

    public void Select(String slot) {
        _equipmentSource = slot;
    }

    public void Apply(Equipment? equipment) {
        if (Ally is null) {
            return;
        }

        var ally = Model.Allies.First(a => a.Id == Ally.Id);
        if (equipment is null && _equipmentSource is not null) {
            equipment = Ally.Equipment[_equipmentSource];
            if (equipment is not null) {
                Model.MoveFromAllyToInventory(equipment, ally, _equipmentSource);
            }
        }
        else if (equipment is not null) {
            Model.MoveFromInventoryToAlly(equipment, ally, equipment.Type);
        }
        else {
            throw new NotSupportedException();
        }
        Select(Model.Allies.First());
    }
}
