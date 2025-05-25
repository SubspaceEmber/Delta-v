using Content.Shared.Dataset;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._DV.Traits.Assorted;

[RegisterComponent, NetworkedComponent]
public sealed partial class HypoesthesiaComponent : Component {
    /// <summary>
    ///     Used for the hypoesthesia trait, which makes a 
    ///     player unable to feel temperature.
    /// </summary>
    [DataField]
    public ProtoId<LocalizedDatasetPrototype> ForceSayNumbDataset = "ForceSayNumbDataset";
}
