using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeepSweeper.UI.MainMenu
{
    public class ButtonsContainer : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Position")]
        [SerializeField] private Vector2 firstButtonPos;
        [SerializeField] private Vector2 buttonOffset;
        [SerializeField] private Vector2 entranceFrom;

        [Header("Timing")]
        [SerializeField] private float entranceTime;
        [SerializeField] private float offsetTime;
        #endregion

        #region Class Members
        private List<MenuButton> buttons;
        private List<RectTransform> buttonRects;
        #endregion

        private void Awake() {
            this.buttons = new List<MenuButton>(GetComponentsInChildren<MenuButton>());
            this.buttonRects = (from btn in GetComponentsInChildren<MenuButton>()
                                select btn.GetComponent<RectTransform>()).ToList();

            //init buttons entrance position
            for (int i = 0; i < buttonRects.Count; i++) {
                RectTransform button = buttonRects[i];
                Vector2 entranceOffset = entranceFrom + buttonOffset * i;
                button.anchoredPosition = entranceOffset;
            }
        }

        private IEnumerator InsertButton(RectTransform rect, Vector3 position, float delay = 0) {
            yield return new WaitForSeconds(delay);

            Vector2 startPos = rect.anchoredPosition;
            float sineWave = Mathf.Sin(1);
            float timer = 0;

            while (timer <= entranceTime) {
                timer += Time.deltaTime;
                float step = timer / entranceTime;
                rect.anchoredPosition = Vector2.Lerp(startPos, position, step);

                yield return null;
            }
        }

        public void InsertButtons() {
            for (int i = 0; i < buttonRects.Count; i++) {
                MenuButton button = buttons[i];
                RectTransform buttonRect = buttonRects[i];
                Vector2 pos = firstButtonPos + (buttonOffset * i);

                MineHeadRotator.Instance.Subscribe(button);
                StartCoroutine(InsertButton(buttonRect, pos, offsetTime * i));
            }
        }
    }
}