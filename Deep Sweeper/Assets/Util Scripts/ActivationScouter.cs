using UnityEngine;

public class ActivationScouter : MonoBehaviour
{
    private bool visible, activated;

    public MineActivator Activator { get; set; }
    public Transform Decider { get; set; }
    public float ReportRange { get; set; }

    private void Update() {
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

    private void OnBecameVisible() {
        activated = InDeciderRange();
        if (activated) Activator?.Activate(true);
        activated = true;
        visible = true;
    }

    private void OnBecameInvisible() {
        Activator?.Activate(false);
        visible = false;
        activated = false;
    }

    /// <returns>True if the scouter's distance from the decider body is within the activation range.</returns>
    private bool InDeciderRange() {
        Vector3 deciderPos = Decider.position;
        Vector3 reporterPos = transform.position;
        float dist = Vector3.Distance(reporterPos, deciderPos);
        return dist <= ReportRange;
    }
}