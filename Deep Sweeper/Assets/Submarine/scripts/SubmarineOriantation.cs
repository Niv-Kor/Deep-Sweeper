using UnityEngine;

public class SubmarineOriantation : MonoBehaviour
{
    private GameFlow flow;

    public Phase CurrentPhase {
        get {
            foreach (Phase phase in flow.Phases) {
                bool entranceOpen = phase.EnteranceGate == null || phase.EnteranceGate.IsOpen;
                bool exitOpen = phase.ExitGate != null && phase.ExitGate.IsOpen;
                if (entranceOpen && !exitOpen) return phase;
            }

            return null; //formal return statement
        }
    }

    private void Start() {
        this.flow = GameFlow.Instance;
    }
}