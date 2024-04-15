using LDJam55.Game.Services;
using Microsoft.AspNetCore.Components;

namespace LDJam55.Game.Pages.Frames;

public partial class ChoiceFrameComponent {
    [Parameter] public required ChoiceFrameAggegrate Frame { get; set; }

    protected override void OnParametersSet() {
        base.OnParametersSet();
    }

    public void SelectOption(String choiceId) {
        Frame.SelectChoice(choiceId);
    }
}
