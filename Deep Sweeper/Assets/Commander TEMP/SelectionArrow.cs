using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class SelectionArrow : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [SerializeField] private float flightTime;
        [SerializeField] private float distance;
        [SerializeField] private float scale;
        #endregion

        #region Constants
        private static readonly Vector2 START_POS = Vector2.zero;
        private static readonly Vector2 START_SCALE = Vector2.zero;
        private static readonly float START_ALPHA = 1;
        #endregion

        #region Class Members
        private RectTransform rect;
        private RawImage sprite;
        #endregion

        #region Properties
        public RadialToolkit.Segment Segment { get; private set; }
        #endregion

        private void Awake() {
            this.rect = GetComponent<RectTransform>();
            this.sprite = GetComponent<RawImage>();
        }

        public void Set(RadialToolkit.Segment segment) {
            Segment = segment;
            Vector2 coordinates = segment.AsCoordinates();
            float xSign = Mathf.Sign(coordinates.x) * -1;
            float z = Vector2.Angle(Vector2.up, coordinates) * xSign;
            rect.rotation = Quaternion.Euler(Vector3.forward * z);
        }

        private IEnumerator Translate() {
            float timer = 0;
            Vector2 origin = rect.anchoredPosition;
            Vector2 dest = Segment.AsCoordinates() * distance;

            while (timer <= flightTime) {
                timer += Time.deltaTime;
                float step = timer / flightTime;
                rect.anchoredPosition = Vector2.Lerp(origin, dest, step);

                yield return null;
            }
        }

        private IEnumerator Resize() {
            float timer = 0;
            Vector2 origin = rect.localScale;
            Vector2 dest = Vector2.one * scale;

            while (timer <= flightTime) {
                timer += Time.deltaTime;
                float step = timer / flightTime;
                rect.localScale = Vector2.Lerp(origin, dest, step);

                yield return null;
            }
        }

        private IEnumerator Colorize() {
            float timer = 0;
            float origin = sprite.color.a;

            while (timer <= flightTime) {
                timer += Time.deltaTime;
                float step = timer / flightTime;
                Color stepColor = sprite.color;
                stepColor.a = Mathf.Lerp(origin, 0, step);
                sprite.color = stepColor;

                yield return null;
            }
        }

        public void Launch() {
            if (Segment == RadialToolkit.Segment.Sector0_0) return;

            //cancel and reset
            StopAllCoroutines();
            rect.anchoredPosition = START_POS;
            rect.localScale = START_SCALE;

            Color arrowColor = sprite.color;
            arrowColor.a = START_ALPHA;
            sprite.color = arrowColor;

            //launch arrow
            StartCoroutine(Translate());
            StartCoroutine(Resize());
            StartCoroutine(Colorize());
        }
    }
}