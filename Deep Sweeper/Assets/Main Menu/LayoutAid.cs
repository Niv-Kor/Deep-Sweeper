using System.Collections;
using UnityEngine;

namespace DeepSweeper.UI.MainMenu
{
    public class LayoutAid : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Entrance Animation")]
        [SerializeField] private float entranceDelay;
        [SerializeField] private float targetWidth;
        [SerializeField] private float entranceTime;

        [Header("Settings")]
        [SerializeField] private bool launchOnAwake;
        #endregion

        #region Class Members
        private RectTransform rect;
        private Coroutine moveCoroutine;
        private ButtonsContainer btnContainer;
        #endregion

        private void Awake() {
            this.rect = GetComponent<RectTransform>();
            this.btnContainer = GetComponentInChildren<ButtonsContainer>();
        }

        private void Start() {
            if (launchOnAwake) Launch();
        }

        private IEnumerator MoveToTarget() {
            yield return new WaitForSeconds(entranceDelay);

            Vector2 baseSize = Vector2.up * rect.sizeDelta.y;
            float startWidth = rect.sizeDelta.x;
            float timer = 0;

            while (timer <= entranceTime) {
                timer += Time.deltaTime;
                float step = timer / entranceTime;
                float stepWidth = Mathf.Lerp(startWidth, targetWidth, step);
                rect.sizeDelta = baseSize + Vector2.right * stepWidth;

                yield return null;
            }

            btnContainer?.InsertButtons();
        }

        private void Launch() {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveToTarget());
        }
    }
}