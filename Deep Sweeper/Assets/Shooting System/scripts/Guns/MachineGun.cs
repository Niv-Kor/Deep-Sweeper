using DeepSweeper.Player.ShootingSystem;

public class MachineGun : PrimaryAutomaticGun
{
    public override GunSubType SubType => GunSubType.MachineGun;

    protected override void FireAtIndicator(TargetInfo target) {
        PullTrigger(transform.forward, target.Grid, true, true);
    }

    protected override void FireAtMine(TargetInfo target) {
        PullTrigger(transform.forward, target.Grid, true, true);
    }

    protected override void FireAtNull() {
        PullTrigger(transform.forward, null, true, true);
    }
}
