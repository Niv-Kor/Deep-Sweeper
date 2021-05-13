using UnityEngine;

namespace DeepSweeper.Player
{
    public class Flashlight : MonoBehaviour
    {
        [Tooltip("The amount of changing intensity positively and negatively.")]
        [SerializeField] private float flickerEpsilon;

        [Tooltip("The amount of light intensity change for each frame.")]
        [SerializeField] private float flickerSpeed;

        private Light lightComponent;
        private float defIntensity;
        private bool flickerPositive;

        private void Start() {
            this.lightComponent = GetComponent<Light>();
            this.defIntensity = lightComponent.intensity;
            this.flickerPositive = true;
            ChangeLightIntensity(-flickerEpsilon / 2);
        }

        private void Update() {
            if (flickerEpsilon <= 0) return;

            int multiplier = flickerPositive ? 1 : -1;
            ChangeLightIntensity(flickerSpeed * multiplier);

            //change flicker direction
            float currentIntensity = lightComponent.intensity;
            bool over = currentIntensity > defIntensity + flickerEpsilon;
            bool under = currentIntensity < defIntensity - flickerEpsilon;
            if (over || under) flickerPositive = !flickerPositive;
        }

        /// <summary>
        /// Change the light's intensity.
        /// </summary>
        /// <param name="delta">Amount of change</param>
        private void ChangeLightIntensity(float delta) {
            lightComponent.intensity += delta;
        }
    }
}