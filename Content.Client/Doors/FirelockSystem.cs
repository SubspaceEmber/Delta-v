using Content.Client.Disposal;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;

namespace Content.Client.Doors;

public sealed class FirelockSystem : SharedFirelockSystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FirelockComponent, AppearanceChangeEvent>(OnAppearanceChange);
    }

    protected override void OnComponentStartup(Entity<FirelockComponent> ent, ref ComponentStartup args)
    {
        base.OnComponentStartup(ent, ref args);
        if(!TryComp<DoorComponent>(ent.Owner, out var door))
            return;

        door.ClosedSpriteStates.Add((DoorVisualLayers.BaseUnlit, ent.Comp.WarningLightSpriteState));
        door.OpenSpriteStates.Add((DoorVisualLayers.BaseUnlit, ent.Comp.WarningLightSpriteState));

        ((Animation)door.OpeningAnimation).AnimationTracks.Add(new AnimationTrackSpriteFlick()
            {
                LayerKey = DoorVisualLayers.BaseUnlit,
                KeyFrames = { new AnimationTrackSpriteFlick.KeyFrame(ent.Comp.OpeningLightSpriteState, 0f) },
            }
        );

        ((Animation)door.ClosingAnimation).AnimationTracks.Add(new AnimationTrackSpriteFlick()
            {
                LayerKey = DoorVisualLayers.BaseUnlit,
                KeyFrames = { new AnimationTrackSpriteFlick.KeyFrame(ent.Comp.ClosingLightSpriteState, 0f) },
            }
        );
    }

    private void OnAppearanceChange(EntityUid uid, FirelockComponent comp, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        var boltedVisible = false;
        var unlitVisible = false;
        // DeltaV begin additions
        var openTemperatureWarningVisible = false;
        var openPressureWarningVisible = false;
        var temperatureWarningVisible = false;
        var pressureWarningVisible = false;
        // DeltaV end additions

        if (!_appearanceSystem.TryGetData<DoorState>(uid, DoorVisuals.State, out var state, args.Component))
            state = DoorState.Closed;
        
        // DeltaV begin additions
        if (!_appearanceSystem.TryGetData<DoorState>(uid, FirelockVisuals.OpenTemperatureWarning, out state, args.Component))
            openTemperatureWarningVisible = state == DoorState.Open;
        if (!_appearanceSystem.TryGetData<DoorState>(uid, FirelockVisuals.OpenPressureWarning, out state, args.Component))
            openPressureWarningVisible = state == DoorState.Open;
        if (!_appearanceSystem.TryGetData<DoorState>(uid, FirelockVisuals.TemperatureWarning, out state, args.Component))
            temperatureWarningVisible = state == DoorState.Closed;
        if (!_appearanceSystem.TryGetData<DoorState>(uid, FirelockVisuals.PressureWarning, out state, args.Component))
            pressureWarningVisible = state == DoorState.Closed;
        // DeltaV end additions

        boltedVisible = _appearanceSystem.TryGetData<bool>(uid, DoorVisuals.BoltLights, out var lights, args.Component) && lights;
        unlitVisible =
            state == DoorState.Closing
            ||  state == DoorState.Opening
            ||  state == DoorState.Denying
            || (_appearanceSystem.TryGetData<bool>(uid, DoorVisuals.ClosedLights, out var closedLights, args.Component) && closedLights);

        args.Sprite.LayerSetVisible(DoorVisualLayers.BaseUnlit, unlitVisible && !boltedVisible);
        args.Sprite.LayerSetVisible(DoorVisualLayers.BaseBolted, boltedVisible);
        // DeltaV Begin additions
        // args.Sprite.LayerSetVisible(FirelockVisualLayersOpenTemperature.Base, openTemperatureWarningVisible);
        // args.Sprite.LayerSetVisible(FirelockVisualLayersOpenPressure.Base, openPressureWarningVisible);
        // args.Sprite.LayerSetVisible(FirelockVisualLayersTemperature.Base, temperatureWarningVisible);
        // args.Sprite.LayerSetVisible(FirelockVisualLayersPressure.Base, pressureWarningVisible);
        // DeltaV end additions
    }
}
