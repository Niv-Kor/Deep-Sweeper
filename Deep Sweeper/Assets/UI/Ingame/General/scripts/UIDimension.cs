﻿using DeepSweeper.Flow;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.UI.Ingame
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIDimension : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("UI Component Settings")]
        [Tooltip("The default time it takes to fade this UI component in or out.")]
        [SerializeField] protected float defaultFadeTime = .5f;

        [Tooltip("True to enable this component as soon as the game starts.")]
        [SerializeField] protected bool EnableOnAwake = false;
        #endregion

        #region Class Members
        protected CanvasGroup canvas;
        private Coroutine fadeCoroutine;
        protected bool m_enabled;
        #endregion

        #region Properties
        public bool Enabled => canvas.alpha > 0;
        #endregion

        protected virtual void Awake() {
            this.canvas = GetComponent<CanvasGroup>();
        }

        protected virtual void Start() {
            Activate(EnableOnAwake, 0);
        }

        /// <summary>
        /// Fade the UI component in or out.
        /// </summary>
        /// <param name="fadeIn">True to fade the component in, or false to fade it out</param>
        /// <param name="time">
        /// The time it will take the fading animation to complete.
        /// Enter -1 to use the default configured time.
        /// </param>
        /// <param name="callback">A callback function to activate after fading in complete</param>
        protected virtual IEnumerator Fade(bool fadeIn, float time, UnityAction callback) {
            time = Mathf.Max(0, (time == -1) ? defaultFadeTime : time);

            float fromVal = canvas.alpha;
            float toVal = fadeIn ? 1 : 0;
            float timer = 0;

            while (timer <= time) {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(fromVal, toVal, timer / time);
                yield return null;
            }

            callback?.Invoke();
        }

        /// <summary>
        /// Show or hide the UI component.
        /// </summary>
        /// <param name="flag">True to show of false to hide</param>
        /// <param name="time">
        /// The time it will take the fading animation to complete.
        /// Enter -1 to use the default configured time.
        /// </param>
        /// <param name="callback">A callback function to activate after animation in complete</param>
        protected virtual void Activate(bool flag, float time = -1, UnityAction callback = null) {
            if (Enabled == flag) return;

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(Fade(flag, time, callback));
        }

        /// <summary>
        /// Reset the spatial's value before a phase starts.
        /// </summary>
        /// <param name="phase">The current phase</param>
        public abstract void ResetValue(Phase phase);

        /// <summary>
        /// Activate when a phase starts.
        /// </summary>
        /// <param name="phase">The starting phase</param>
        public abstract void OnPhaseStarts(Phase phase);

        /// <summary>
        /// Activate when a phase is paused.
        /// </summary>
        /// <param name="phase">The paused phase</param>
        public abstract void OnPhasePauses(Phase phase);

        /// <summary>
        /// Activate when a phase resumes after a pause.
        /// </summary>
        /// <param name="phase">The resumed phase</param>
        public abstract void OnPhaseResumes(Phase phase);

        /// <summary>
        /// Activate when a phase is finished.
        /// </summary>
        /// <param name="phase">The finished phase</param>
        /// <param name="success">True if the phase finished successfully</param>
        public abstract void OnPhaseEnds(Phase phase, bool success);
    }
}