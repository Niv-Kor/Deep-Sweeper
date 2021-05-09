namespace DeepSweeper.ShootingSystem
{
    public abstract class SecondarySubmarineGun : SubmarineGun
    {
        #region Properties
        public override GunType Type => GunType.Secondary;
        #endregion
    }
}