using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LifeBit : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Timing")]
    [Tooltip("The time it takes the icon to scale up once (in seconds).")]
    [SerializeField] private float scaleUpTime;

    [Tooltip("The time it takes the icon to scale down once (in seconds).")]
    [SerializeField] private float scaleDownTime;
    #endregion

    #region Class Members
    private bool activated;
    #endregion

    private void Awake() {
        this.activated = false;
    }

    /// <summary>
    /// Scale the life bit up (from 0) or down (to 0).
    /// </summary>
    /// <param name="scaleUp">True to scale the life bit up</param>
    /// <param name="callback">A callback function to activate when the scale is done</param>
    private IEnumerator Scale(bool scaleUp, UnityAction callback = null) {
        Vector3 fromVec = scaleUp ? Vector3.zero : Vector3.one;
        Vector3 toVec = scaleUp ? Vector3.one : Vector3.zero;
        float time = scaleUp ? scaleUpTime : scaleDownTime;
        float timer = 0;

        while (timer <= time) {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(fromVec, toVec, timer / time);
            yield return null;
        }

        callback?.Invoke();
    }

    /// <summary>
    /// Activate the life bit's icon.
    /// This method does not affect the life bit's functionality,
    /// but only the lets the user decide if it should be animated
    /// or instantly appear.
    /// A life bit that does not use this method will not appear at all in the UI.
    /// </summary>
    /// <param name="animate">True to animate the icon's scaling</param>
    public void Activate(bool animate) {
        if (animate) StartCoroutine(Scale(true));
        else transform.localScale = Vector3.one;

        activated = true;
    }

    /// <summary>
    /// Dispose the life bit.
    /// </summary>
    /// <param name="animate">True to animate the icon's scaling</param>
    public void Dispose(bool animate) {
        void DestroyBit() { Destroy(gameObject); }

        if (activated && animate) StartCoroutine(Scale(false, DestroyBit));
        else DestroyBit();
    }
}