using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Level.Mine
{
    public class SensorsColorizer : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Timing")]
        [Tooltip("The time it takes to set the color of the sensors [s].")]
        [SerializeField] private float colorSetTime = .5f;

        [Tooltip("The time in between each two bright pulses [s].")]
        [SerializeField] private float pulseDuration = 1;
        #endregion

        #region Constants
        private static readonly string SHADER_HIGHLIGHT_VALUE_NAME = "_HighlitColor";
        private static readonly string SHADER_DARK_VALUE_NAME = "_DarkColor";
        private static readonly float DARK_COLOR_ADJUSTMENT_PERCENT = .5f;
        #endregion

        #region Class Members
        private List<Material> materials;
        private SensorsManager mngr;
        private UnityAction interval;
        private float pulseTimer;
        #endregion

        #region Events
        private event UnityAction PulseEvent;
        #endregion

        private void Awake() {
            this.pulseTimer = 0;
            this.mngr = GetComponent<SensorsManager>();

            List<Renderer> renderers = (from sensor in mngr.Sensors
                                        select sensor.GetComponent<Renderer>()).ToList();

            this.materials = (from renderer in renderers
                              select renderer.material).ToList();
        }

        private void Update() {
            pulseTimer += Time.deltaTime;

            if (pulseTimer >= pulseDuration) {
                pulseTimer = 0;
                PulseEvent?.Invoke();
            }
        }

        /// <summary>
        /// Darken a color by a specific rate.
        /// </summary>
        /// <param name="color">The color to darken</param>
        /// <param name="rate">
        /// The percentage at which the color should be darkened [0:1],
        /// where 1 is completely black
        /// </param>
        /// <returns>The darkened color.</returns>
        private Color DarkenColor(Color color, float rate) {
            float r = color.r;
            float g = color.g;
            float b = color.b;
            r -= r * rate;
            g -= g * rate;
            b -= b * rate;
            return new Color(r, g, b);
        }

        /// <summary>
        /// Change the color of the sensors.
        /// </summary>
        /// <param name="color">Target color</param>
        /// <param name="time">The time it takes to change the sensors' color</param>
        /// <param name="callback">A callback function to be activated once the process is complete</param>
        private IEnumerator ChangeSensorsColor(Color color, float time, UnityAction callback = null) {
            float timer = 0;
            Color darkened = DarkenColor(color, DARK_COLOR_ADJUSTMENT_PERCENT);
            IDictionary<Material, Color> srcColors = new Dictionary<Material, Color>();

            //save source colors
            foreach (Material material in materials) {
                Color srcColor = material.GetColor(SHADER_HIGHLIGHT_VALUE_NAME);
                srcColors.Add(material, srcColor);
            }

            while (timer <= time) {
                timer += Time.deltaTime;
                float step = timer / time;

                //lerp
                foreach (Material material in materials) {
                    srcColors.TryGetValue(material, out Color srcColor);
                    material.SetColor(SHADER_HIGHLIGHT_VALUE_NAME, Color.Lerp(srcColor, color, step));
                    material.SetColor(SHADER_DARK_VALUE_NAME, Color.Lerp(srcColor, darkened, step));
                }

                yield return null;
            }

            callback?.Invoke();
        }

        /// <summary>
        /// Colorize the sensors.
        /// </summary>
        /// <param name="color">The target color of the sensors</param>
        public void Colorize(Color color) {
            //remove last interval
            if (interval != null) PulseEvent -= interval;
            StopAllCoroutines();

            //set first color quickly
            StartCoroutine(ChangeSensorsColor(color, colorSetTime, delegate {
                //assign new interval
                interval = delegate {
                    StopAllCoroutines();
                    StartCoroutine(ChangeSensorsColor(color, pulseDuration));
                };

                PulseEvent += interval;
            }));
        }
    }
}