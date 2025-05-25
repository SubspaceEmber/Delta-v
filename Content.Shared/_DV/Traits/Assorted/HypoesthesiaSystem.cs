using Content.Shared.Damage.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Events;
using Content.Shared.Mobs.Systems;

namespace Content.Shared._DV.Traits.Assorted;

public sealed class HypoesthesiaSystem : EntitySystem
{
    [Dependency] private readonly MobThresholdSystem _mobThresholdSystem = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<HypoesthesiaComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<HypoesthesiaComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<HypoesthesiaComponent, BeforeForceSayEvent>(OnChangeForceSay);
        SubscribeLocalEvent<HypoesthesiaComponent, BeforeAlertSeverityCheckEvent>(OnAlertSeverityCheck);
    }

    private void OnComponentRemove(EntityUid uid, HypoesthesiaComponent component, ComponentRemove args)
    {
        if (!HasComp<MobThresholdsComponent>(uid))
            return;

        _mobThresholdSystem.VerifyThresholds(uid);
    }

    private void OnComponentInit(EntityUid uid, HypoesthesiaComponent component, ComponentInit args)
    {
        if (!HasComp<MobThresholdsComponent>(uid))
            return;

        _mobThresholdSystem.VerifyThresholds(uid);
    }
    
    private void OnChangeForceSay(Entity<HypoesthesiaComponent> ent, ref BeforeForceSayEvent args)
    {
        args.Prefix = ent.Comp.ForceSayNumbDataset;
    }

    private void OnAlertSeverityCheck(Entity<HypoesthesiaComponent> ent, ref BeforeAlertSeverityCheckEvent args)
    {
        if (args.CurrentAlert == "Hot")
            args.CancelUpdate = true;
        if (args.CurrentAlert == "Cold")
            args.CancelUpdate = true;
    }
}