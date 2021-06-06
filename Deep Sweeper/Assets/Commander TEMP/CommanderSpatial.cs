using DeepSweeper.CameraSet;
using DeepSweeper.Characters;
using DeepSweeper.Flow;
using DeepSweeper.Player.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class CommanderSpatial : Spatial
    {
        #region Expsoed Editor Parameters
        [SerializeField] private List<SpriteConfiguration> commanders;

        [SerializeField] private float sensitivity = 1;

        [SerializeField] private List<CharacterPersona> Characters;

        [SerializeField] private SectorManager sector0_120;

        [SerializeField] private SectorManager sector120_240;

        [SerializeField] private SectorManager sector240_360;
        #endregion

        #region Constants
        private static readonly int DEFAULT_COMMANDER_INDEX = 0;
        #endregion

        #region Class Members
        private SectorialDivisor sectorialDivisor;
        private SectorManager currentSector;
        private SectorManager lastSelected;
        private PlayerController controls;
        private Coroutine fadeCoroutine;
        #endregion

        #region Events
        /// <param type=typeof(CharacterPersona)>The changed character</param>
        /// <param type=typeof(CharacterPersona)>The new character</param>
        public event UnityAction<CharacterPersona, CharacterPersona> CommanderChangedEvent;
        #endregion

        protected override void Awake() {
            base.Awake();
            this.controls = PlayerController.Instance;
            this.sectorialDivisor = GetComponentInChildren<SectorialDivisor>();
        }

        protected override void Start() {
            base.Start();
            
            transform.localScale = Vector3.zero;
            sectorialDivisor.PopulateCharacters(Characters);

            //bind events
            controls.CommanderSelectionStartEvent += OnSelectionKeyDown;
            controls.CommanderSelectionEndEvent += OnSelectionKeyUp;
        }

        /// <summary>
        /// Activate when the player presses the selection key.
        /// This function enables the selection of a commander.
        /// </summary>
        private void OnSelectionKeyDown() {
            void EnableSelection() { controls.MouseMoveEvent += OnMouseMovement; }
            Activate(true, -1, EnableSelection);
        }

        /// <summary>
        /// Activate when the player releases the selection key.
        /// This function disables the selection of a commander.
        /// </summary>
        private void OnSelectionKeyUp() {
            SelectCharacter();
            Activate(false);
            controls.MouseMoveEvent -= OnMouseMovement;
        }

        /// <summary>
        /// Activate when the mouse moves to any direction.
        /// This function checks the direction of the mouse and selects
        /// the correct character sector accordingly.
        /// </summary>
        /// <param name="delta">The movement vector of the mouse during the last frame</param>
        private void OnMouseMovement(Vector2 delta) {
            float x = delta.x;
            float y = delta.y;

            if (x > sensitivity && y >= 0) SelectSector(sector0_120);
            else if (y < -sensitivity) SelectSector(sector120_240);
            else if (x < -sensitivity && y >= 0) SelectSector(sector240_360);
        }

        /// <summary>
        /// Select a character section.
        /// </summary>
        /// <param name="sector">The sector to select</param>
        private void SelectSector(SectorManager sector) {
            if (currentSector != null) currentSector.Selected = false;
            sector.Selected = true;
            currentSector = sector;
        }

        /// <summary>
        /// Select the character with the highlighter sector.
        /// </summary>
        private void SelectCharacter() {
            if (currentSector is null) return;

            var prevChar = (lastSelected is null) ? CharacterPersona.None : lastSelected.Character;
            var nextChar = currentSector.Character;
            lastSelected = currentSector;

            CommanderChangedEvent?.Invoke(prevChar, nextChar);
        }

        /// <summary>
        /// Subscribe to a commander change event.
        /// </summary>
        /// <param name="listener">The listener to activate when a commander changes</param>
        /// <returns>The default first commander on level startup.</returns>
        public CharacterPersona SubscribeToCommanderChange(UnityAction<CharacterPersona, CharacterPersona> listener) {
            CommanderChangedEvent += listener;
            return commanders[DEFAULT_COMMANDER_INDEX].Character;
        }

        /// <inheritdoc/>
        protected override IEnumerator Switch(bool switchIn, float time, UnityAction callback) {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(base.Switch(switchIn, time, null));

            time = Mathf.Max(0, (time == -1) ? defaultFadeTime : time);
            Vector3 from = transform.localScale;
            Vector3 to = switchIn ? Vector3.one : Vector3.zero;
            float timer = 0;

            while (timer <= time) {
                timer += Time.deltaTime;
                transform.localScale = Vector3.Lerp(from, to, timer / time);
                yield return null;
            }

            callback?.Invoke();
        }

        /// <inheritdoc/>
        protected override bool Activate(bool flag, float time = -1, UnityAction callback = null) {
            bool success = base.Activate(flag, time, callback);
            if (!success) return false;

            CameraRig rig = CameraManager.Instance.GetRig(CameraRole.Main);
            if (flag) rig.Pause(true);
            else rig.Resume(true);

            return true;
        }

        /// <inheritdoc/>
        public override void ResetValue(Phase phase) {}

        /// <inheritdoc/>
        public override void OnPhaseStarts(Phase phase) {}

        /// <inheritdoc/>
        public override void OnPhasePauses(Phase phase) { Activate(false); }

        /// <inheritdoc/>
        public override void OnPhaseResumes(Phase phase) {}

        /// <inheritdoc/>
        public override void OnPhaseEnds(Phase phase, bool success) { Activate(false); }
    }
}