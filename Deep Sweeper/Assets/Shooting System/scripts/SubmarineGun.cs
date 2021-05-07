using UnityEngine;

public abstract class SubmarineGun : MonoBehaviour
{
    protected enum GunType
    {
        Primary,
        Secondary
    }

    #region Properties
    protected abstract GunType Type { get; }
    #endregion

    protected virtual void Start() {
        //bind fire events
        SightRay.Instance.PrimaryHitEvent += delegate(SightRay.SightTargetType targetType, SightRay.TargetInfo target) {
            if (Type == GunType.Primary) OperateTarget(targetType, target);
        };

        SightRay.Instance.SecondaryHitEvent += delegate(SightRay.SightTargetType targetType, SightRay.TargetInfo target) {
            if (Type == GunType.Secondary) OperateTarget(targetType, target);
        };
    }

    /// <summary>
    /// respond to the a shoot.
    /// </summary>
    /// <param name="targetType">Type of target</param>
    /// <param name="target">Target info</param>
    protected virtual void OperateTarget(SightRay.SightTargetType targetType, SightRay.TargetInfo target) {
        if (target == null) {
            FireAtNull();
            return;
        }
        else {
            switch (targetType) {
                case SightRay.SightTargetType.Mine: FireAtMine(target); break;
                case SightRay.SightTargetType.Indicator: FireAtIndicator(target); break;
                default: FireAtNull(); break;
            }
        }
    }

    /// <summary>
    /// Fire at a grid indicator.
    /// </summary>
    /// <param name="target">Target grid's info (never null)</param>
    protected abstract void FireAtIndicator(SightRay.TargetInfo target);

    /// <summary>
    /// Fire at a mine.
    /// </summary>
    /// <param name="target">Target grid's info (never null)</param>
    protected abstract void FireAtMine(SightRay.TargetInfo target);

    /// <summary>
    /// Activate when the gun shoots at a null target.
    /// </summary>
    protected abstract void FireAtNull();
}