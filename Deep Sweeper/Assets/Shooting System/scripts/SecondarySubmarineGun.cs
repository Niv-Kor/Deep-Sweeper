public class SecondarySubmarineGun : SubmarineGun
{
    #region Properties
    protected override GunType Type => GunType.Secondary;
    #endregion

    protected override void FireAtIndicator(SightRay.TargetInfo target) {}

    protected override void FireAtMine(SightRay.TargetInfo target) {
        target.Selector.ToggleFlag();
    }

    protected override void FireAtNull() {}
}