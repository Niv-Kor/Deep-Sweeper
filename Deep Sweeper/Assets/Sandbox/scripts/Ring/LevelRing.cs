using DeepSweeper.Flow;
using GamedevUtil.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Menu.UI.Campaign.Sandbox.Ring
{
    public class LevelRing : MonoBehaviour
    {
        private enum RingState
        {
            Allowed,
            Forbidden,
            Hovered,
            Selected
        }

        [System.Serializable]
        private struct RingStateColor
        {
            [Tooltip("The state of the ring.")]
            [SerializeField] public RingState State;

            [Tooltip("The color of the ring while at the specified state.")]
            [SerializeField] public Color Color;
        }

        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("A list of the particle systems to be colorized upon mouse events.")]
        [SerializeField] private List<ParticleSystemRenderer> colorizableParticles;

        [Header("Level")]
        [Tooltip("A list of the ring's states color configurations.")]
        [SerializeField] private List<RingStateColor> colorConfig;

        [Tooltip("The time it takes the color of the particles to change (in seconds).")]
        [SerializeField] private float colorLerpTime;
        #endregion

        #region Constants
        private static readonly Color DEFAULT_STATE_COLOR = Color.black;
        #endregion

        #region Class Members
        private bool m_selected;
        private RingState defaultState;
        private RingState currentState;
        private SandboxLevel level;
        #endregion

        #region Events
        /// <param bool>True if the ring is selected</param>
        public event UnityAction<bool> SelectedEvent;
        public event UnityAction InitializedEvent;
        #endregion

        #region Properties
        public RingsManager Manager { get; set; }
        public Region Region { get; private set; }
        public bool Selected {
            get => m_selected;
            set {
                if (value != m_selected) {
                    RingState nextState = value ? RingState.Selected : defaultState;

                    if (value) Manager.ReportSelection(this);
                    StartCoroutine(ApplyState(nextState));
                    m_selected = value;
                    SelectedEvent?.Invoke(value);
                }
            }
        }
        #endregion

        private void Awake() {
            this.m_selected = false;
            this.defaultState = RingState.Allowed;
            this.currentState = defaultState;
        }

        private void Start() {
            this.level = GetComponentInParent<SandboxLevel>();
            this.Region = level.Region;

            //edit region name
            RingRegionLabel[] labels = GetComponentsInChildren<RingRegionLabel>();
            var filter = new UIRegionNameFilter();

            foreach (var label in labels)
                label.Text = EnumNameFilter<Region>.Filter(Region, filter);

            void reportInitEvent() { InitializedEvent?.Invoke(); }
            StartCoroutine(ApplyState(currentState, reportInitEvent));
        }

        private void OnValidate() {
            //confirm every state appears in the color config list
            foreach (RingState state in System.Enum.GetValues(typeof(RingState))) {
                if (colorConfig.FindIndex(x => x.State == state) == -1) {
                    RingStateColor stateColor;
                    stateColor.State = state;
                    stateColor.Color = DEFAULT_STATE_COLOR;
                }
            }
        }

        /// <inheritdoc/>
        public void OnMouseDown() { Selected = true; }

        /// <inheritdoc/>
        public void OnMouseEnter() {
            if (!Selected) StartCoroutine(ApplyState(RingState.Hovered));
        }

        /// <inheritdoc/>
        public void OnMouseExit() {
            if (!Selected) StartCoroutine(ApplyState(defaultState));
        }

        /// <param name="state">The state of which to get the color configuration</param>
        /// <returns>The color configuration of the specified state.</returns>
        private Color GetColorConfig(RingState state) {
            return colorConfig.Find(x => x.State == state).Color;
        }

        /// <summary>
        /// Apply a ring's state.
        /// </summary>
        /// <param name="state">The state to apply</param>
        private IEnumerator ApplyState(RingState state, UnityAction callback = null) {
            float timer = 0;
            Color color = GetColorConfig(state);
            currentState = state;

            //save source colors
            IDictionary<ParticleSystemRenderer, Color> srcColors = new Dictionary<ParticleSystemRenderer, Color>();
            foreach (var particles in colorizableParticles) srcColors.Add(particles, particles.material.color);

            while (timer <= colorLerpTime) {
                timer += Time.deltaTime;

                foreach (var particles in colorizableParticles) {
                    float step = timer / colorLerpTime;
                    srcColors.TryGetValue(particles, out Color srcColor);
                    particles.material.color = Color.Lerp(srcColor, color, step);
                }

                yield return null;
            }

            callback?.Invoke();
        }
    }
}