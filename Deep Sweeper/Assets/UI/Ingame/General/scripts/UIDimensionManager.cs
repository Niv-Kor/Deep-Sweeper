using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DeepSweeper.Flow;

namespace DeepSweeper.UI.Ingame
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIDimensionManager<T, DIM> : Singleton<T> where T : MonoBehaviour where DIM : UIDimension
    {
        #region Exposed Editor Parameters
        [Tooltip("The default time it takes to fade this entire UI dimension in or out (in seconds).")]
        [SerializeField] protected float defaultFadeTime;
        #endregion

        #region Class Members
        protected CanvasGroup canvas;
        protected Coroutine fadeCoroutine;
        protected List<DIM> components;
        #endregion

        protected override void Awake() {
            base.Awake();

            this.canvas = GetComponent<CanvasGroup>();
            this.components = new List<DIM>(GetComponentsInChildren<DIM>());
            LevelFlow flow = LevelFlow.Instance;

            //bind events
            flow.PhaseStartEvent += OnPhaseStart;
            flow.PhasePauseEvent += OnPhasePause;
            flow.PhaseResumeEvent += OnPhaseResume;
            flow.PhaseEndEvent += OnPhaseEnd;
        }

        /// <summary>
        /// Fade the canvas group in or out.
        /// </summary>
        /// <param name="fadeIn">True to fade the spatials in, or false to fade them out</param>
        /// <param name="time">
        /// The time it will take the fading animation to complete.
        /// Enter -1 to use the default configured time.
        /// </param>
        /// <param name="callback">A function to activate after fading in complete</param>
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
        /// Activate when a phase starts.
        /// </summary>
        /// <see cref="LevelFlow.PhaseStartEvent"/>
        protected virtual void OnPhaseStart(Phase phase) {
            foreach (DIM component in components) {
                component.ResetValue(phase);
                component.OnPhaseStarts(phase);
            }
        }

        /// <summary>
        /// Activate when a phase pauses.
        /// </summary>
        /// <see cref="LevelFlow.PhasePauseEvent"/>
        protected virtual void OnPhasePause(Phase phase) {
            foreach (DIM component in components)
                component.OnPhasePauses(phase);
        }

        /// <summary>
        /// Activate when a phase resumes.
        /// </summary>
        /// <see cref="LevelFlow.PhaseResumeEvent"/>
        protected virtual void OnPhaseResume(Phase phase) {
            foreach (DIM component in components)
                component.OnPhaseResumes(phase);
        }

        /// <summary>
        /// Activate when a phase ends.
        /// </summary>
        /// <see cref="LevelFlow.PhaseEndEvent"/>
        protected virtual void OnPhaseEnd(Phase phase, bool success) {
            foreach (DIM component in components)
                component.OnPhaseEnds(phase, success);
        }

        /// <summary>
        /// Activate or deactivate the UI components.
        /// </summary>
        /// <param name="flag">True to activate or false to deactivate</param>
        /// <param name="time">
        /// The time it will take the fading animation to complete.
        /// Enter -1 to use the default configured time.
        /// </param>
        /// <param name="callback">A function to activate after the fade completes</param>
        public virtual void Activate(bool flag, float time = -1, UnityAction callback = null) {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(Fade(flag, time, callback));
        }

        /// <param name="classType">A class type of a UI component</param>
        /// <returns>The requested UI component, or null if it doesn't exist under this manager.</returns>
        public DIM Get(Type classType) {
            return components.Find(x => x.GetType() == classType);
        }
    }
}