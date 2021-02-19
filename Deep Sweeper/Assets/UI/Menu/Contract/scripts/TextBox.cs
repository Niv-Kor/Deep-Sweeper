using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu.Contract
{
    public class TextBox : MonoBehaviour, IPointerClickHandler
    {
        #region Exposed Editor Parameters
        [Header("Timing")]
        [Tooltip("The time it takes to write one character.")]
        [SerializeField] private float typeTime = .5f;
        #endregion

        #region Constants
        private static readonly Color TRANSPARENT = new Color(0x0, 0x0, 0x0, 0);
        #endregion

        #region Class Members
        private TextMeshProUGUI textCmp;
        private Color textColor;
        private string buffer;
        private List<TMP_PageInfo> pageInfo;
        private bool[] readHistory;
        #endregion

        #region Properties
        public int Page { get; private set; }
        public int PageCount { get; private set; }
        #endregion

        private void Start() {
            this.textCmp = GetComponentInChildren<TextMeshProUGUI>();
            this.textColor = textCmp.color;
            this.buffer = "";
            this.Page = 0;

            LoadText("ddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdssdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdssdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdssdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdssdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsd sdsdsddsdds dsdsddsddsdsdsddsddsdsd sddsddsd sd sddsddsdsdsdds ddsdsdsdd sddsdsds jhgjhgjhgjtyjtddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsdsdsdsddsdds dsdsddsddsdsdsddsddsdsd sddsddsd sd sddsddsdsdsdds ddsdsdsdd sddsdsds jhgjhgjhgjtyjtddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsddsdsdsddsd sdsdsddsdds dsdsddsddsdsdsddsddsdsd sddsddsd sd sddsddsdsdsdds ddsdsdsdd sddsdsds jhgjhgjhgjtyjt");
        }

        /// <summary>
        /// Write the text in the text box character by character.
        /// </summary>
        private IEnumerator Write(int page, float characterTime) {
            int readIndex = pageInfo[page].firstCharacterIndex;
            int contentSize = pageInfo[Page].lastCharacterIndex;

            if (characterTime > 0) {
                while (readIndex <= contentSize) {
                    textCmp.text += buffer[readIndex++];
                    yield return new WaitForSeconds(characterTime);
                }
            }
            else textCmp.text = buffer.Substring(readIndex, contentSize - readIndex);
        }

        public void ClearCurrentBoard() {
            StopAllCoroutines();
            textCmp.text = "";
        }

        /// <summary>
        /// Load text into the text box.
        /// </summary>
        /// <param name="text">The text to load</param>
        public void LoadText(string text, bool instant = false) {
            //fake render text
            textCmp.color = TRANSPARENT;
            textCmp.text = text;

            StopAllCoroutines();
            StartCoroutine(CountPagesAndLoad(text, instant));
        }

        private IEnumerator CountPagesAndLoad(string text, bool instant = false) {
            //skip one frame
            yield return null;

            //count pages
            PageCount = textCmp.textInfo.pageCount;
            pageInfo = new List<TMP_PageInfo>(textCmp.textInfo.pageInfo);
            textCmp.text = "";
            textCmp.color = textColor;
            readHistory = new bool[PageCount];

            buffer = text;
            float time = instant ? 0 : typeTime;
            yield return Write(0, time);
        }

        /// <inheritdoc/>
        public void OnPointerClick(PointerEventData ev) {
            StopAllCoroutines();
            StartCoroutine(Write(Page, 0));
        }

        private void ChangePage(int page) {
            if (page == Page || page < 0 || page >= PageCount) return;
            else Page = page;

            ClearCurrentBoard();
            bool instant = readHistory[Page];
            float time = instant ? 0 : typeTime;
            readHistory[Page] = true;
            StartCoroutine(Write(Page, time));
        }

        public void PrevPage() {
            ChangePage(Page - 1);
        }

        public void NextPage() {
            ChangePage(Page + 1);
        }
    }
}