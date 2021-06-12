using DeepSweeper.Characters;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class CooldownCurtain : MonoBehaviour, IDynamicSectorComponent
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The component of the character's sprite.")]
        [SerializeField] private RawImage characterSprite;

        [Tooltip("The timer's text component.")]
        [SerializeField] private TextMeshProUGUI countdownTimer;

        [Header("Character Color")]
        [Tooltip("The color of the character while the cooldown is on.")]
        [SerializeField] private Color grayedCharacterColor;

        [Tooltip("The time it takes the character's color to restore "
               + "after the cooldown is over [s].")]
        [SerializeField] private float characterRestoreTime;

        [Header("Time")]
        [Tooltip("The default cooldown time when not set manually [s].")]
        [SerializeField] private int defaultCooldown;

        [Tooltip("The distance of the timer text from the center of the circle, "
               + "as a percentage of the circle's radius.")]
        [SerializeField] [Range(0f, 1f)] private float timerDistance;
        #endregion

        #region Class Members
        private Image sprite;
        private RectTransform rect;
        private Color originCharColor;
        private float targetFill;
        #endregion

        #region Properties
        public bool Running { get; private set; }
        public int TimeLeft { get; private set; }
        public float Percentage {
            get => 1 - RangeMath.NumberOfRange(sprite.fillAmount, new Vector2(targetFill, 1));
            set { sprite.fillAmount = targetFill; }
        }
        #endregion

        private void Awake() {
            this.sprite = GetComponent<Image>();
            this.rect = GetComponent<RectTransform>();
            this.originCharColor = characterSprite.color;
            this.Running = false;
        }

        private void OnValidate() {
            defaultCooldown = Mathf.Max(0, defaultCooldown);
        }

        /// <summary>
        /// Activate the timer's text countdown.
        /// </summary>
        /// <param name="time">The timer's start time [s]</param>
        private IEnumerator Countdown(int time) {
            TimeLeft = time;

            while (TimeLeft > 0) {
                if (Running) {
                    countdownTimer.text = TimeLeft--.ToString();
                    yield return new WaitForSeconds(1);
                }

                yield return null;
            }

            countdownTimer.text = string.Empty;
        }

        /// <summary>
        /// Slowly empty the cooldown's mask from the sector.
        /// </summary>
        /// <param name="time">The time it takes the process to finish</param>
        private IEnumerator OpenCurtain(float time) {
            float timer = 0;

            while (timer <= time) {
                if (Running) {
                    timer += Time.deltaTime;
                    sprite.fillAmount = Mathf.Lerp(1, targetFill, timer / time);
                }

                yield return null;
            }

            Running = false;
            yield return RestoreCharacterColor();
        }

        /// <summary>
        /// Slowly restore the color of the character
        /// from when the sector had not been in cooldown.
        /// </summary>
        private IEnumerator RestoreCharacterColor() {
            Color startColor = characterSprite.color;
            float timer = 0;

            while (timer <= characterRestoreTime) {
                timer += Time.deltaTime;
                float step = timer / characterRestoreTime;
                characterSprite.color = Color.Lerp(startColor, originCharColor, step);

                yield return null;
            }
        }

        /// <inheritdoc/>
        public void Build(SegmentInstructions instructions, Persona character = Persona.None) {
            RadialToolkit.Segment segment = instructions.Segment;
            RadialToolkit.RadialDivision division = segment.Originate();
            float angleDeviation = instructions.Roll;

            //fix components deviation
            if (Mathf.Abs(angleDeviation) > 0)
                rect.Rotate(0, 0, -angleDeviation);

            //set radial fill position and amount
            targetFill = 1 - (1f / division.AsAmount());
            sprite.fillClockwise ^= true;

            //position timer
            Vector3 fixedTimerDistance = timerDistance * rect.sizeDelta / 2;
            Vector3 timerPos = Vector3.Scale(segment.AsCoordinates(), fixedTimerDistance);
            countdownTimer.rectTransform.anchoredPosition = timerPos;
        }

        /// <summary>
        /// Start the cooldown.
        /// </summary>
        /// <param name="time">
        /// Cooldown time (defaults to the fixed
        /// 'defaultCooldown' parameter when empty)
        /// </param>
        public void Begin(int time = -1) {
            time = (time == -1) ? defaultCooldown : Mathf.Max(0, time);

            Cancel();
            Running = true;
            characterSprite.color = grayedCharacterColor;
            StartCoroutine(OpenCurtain(time));
            StartCoroutine(Countdown(time));
        }

        /// <summary>
        /// Pause the cooldown.
        /// It will not be stopped entirely.
        /// </summary>
        /// <seealso cref="Resume"/>
        public void Pause() { Running = false; }

        /// <summary>
        /// Resume the cooldown after a pause.
        /// </summary>
        public void Resume() {
            if (TimeLeft > 0) Running = true;
        }

        /// <summary>
        /// Cancel the cooldown entirely.
        /// </summary>
        public void Cancel() {
            StopAllCoroutines();
            TimeLeft = 0;
            Percentage = 0;
        }
    }
}