using UnityEngine;

public abstract class ObjectActivator : MonoBehaviour
{
    protected bool lockFlag;

    /// <summary>
    /// Activate or deactivate the object.
    /// This method does not work if the activation state is locked,
    /// so it must first be unlocked.
    /// </summary>
    /// <param name="flag">True to enable or false to disable</param>
    /// <returns>True if the activation is successful.</returns>
    public bool Activate(bool flag) {
        if (!lockFlag) Enable(flag);
        return !lockFlag;
    }

    /// <summary>
    /// Activate or deactivate the object.
    /// This method is the implementaion of the 'Activate' method,
    /// so it must be overriden.
    /// </summary>
    /// <param name="flag">True to enable or false to disable</param>
    protected abstract void Enable(bool flag);

    /// <summary>
    /// Activate or deactivate the object, while also locking this state.
    /// If another 'Activate' method is called after this lock, it will be ignored.
    /// </summary>
    public virtual void ActivateAndLock() {
        lockFlag = true;
        Enable(true);
    }

    /// <summary>
    /// Unlock the activation state and allow the 'Activate' method to be called.
    /// </summary>
    public virtual void Unlock() {
        lockFlag = false;
    }
}