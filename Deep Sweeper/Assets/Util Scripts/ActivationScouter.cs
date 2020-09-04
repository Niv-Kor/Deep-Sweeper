using UnityEngine;

public class ActivationScouter : MonoBehaviour
{
    protected bool visible, activated;

    public ObjectActivator Activator { get; set; }
    public Transform Decider { get; set; }
    public float ReportRange { get; set; }

    protected virtual void Start() {
        this.visible = false;
        this.activated = false;
    }

    protected virtual void Update() {
        if (visible) {
            bool inRange = InDeciderRange();

            if (activated && !inRange) {
                activated = !Activator.Activate(false);
            }
            else if (!activated && inRange) {
                activated = Activator.Activate(true);
            }
        }
    }

    protected virtual void OnBecameVisible() {
        activated = InDeciderRange();
        if (activated) Activator?.Activate(true);
        activated = true;
        visible = true;
    }

    protected virtual void OnBecameInvisible() {
        Activator?.Activate(false);
        visible = false;
        activated = false;
    }

    /// <returns>True if the scouter's distance from the decider body is within the activation range.</returns>
    protected virtual bool InDeciderRange() {
        Vector3 deciderPos = Decider.position;
        Vector3 reporterPos = transform.position;
        float dist = Vector3.Distance(reporterPos, deciderPos);
        return dist <= ReportRange;
    }
}