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
        [SerializeField] private List<Persona> Characters;
        #endregion

        #region Constants
        private static readonly float CHANGE_COOLDOWN = .1f;
        #endregion

        #region Class Members
        private SectorialDivisor sectorialDivisor;
        private PlayerController controls;
        private Coroutine cooldownCoroutine;
        private Coroutine fadeCoroutine;
        private Delimeter delimeter;
        private Persona lastSelected;
        private bool changeable;
        #endregion

        #region Events
        /// <param type=typeof(CharacterPersona)>The changed character</param>
        /// <param type=typeof(CharacterPersona)>The new character</param>
        public event UnityAction<Persona, Persona> CommanderChangedEvent;
        #endregion

        #region Properties
        private int DefaultCommanderIndex => 0;
        private Persona DefaultCommander {
            get {
                int defIndex = DefaultCommanderIndex;
                bool exists = defIndex >= 0;
                return exists ? Characters[defIndex] : Persona.None;
            }
        }
        #endregion

        protected override void Awake() {
            base.Awake();
            this.controls = PlayerController.Instance;
            this.delimeter = GetComponentInChildren<Delimeter>();
            this.sectorialDivisor = GetComponentInChildren<SectorialDivisor>();
            this.changeable = true;
        }

        protected override void Start() {
            base.Start();
            
            transform.localScale = Vector3.zero;
            sectorialDivisor.PopulateCharacters(Characters);
            delimeter.Build(Characters.Count);
            SelectCharacter(DefaultCommander);

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
            if (!changeable || !sectorialDivisor.NavigateToSector(delta)) return;

            changeable = false;
            if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = StartCoroutine(RunChangeCooldown());
        }

        /// <summary>
        /// Select the character with the highlighter sector.
        /// </summary>
        private void SelectCharacter() {
            SectorManager current = sectorialDivisor.CurrentSector;
            if (current != null) SelectCharacter(current.Character);
        }

        private void SelectCharacter(Persona character) {
            sectorialDivisor.SelectSector(character);

            Persona prevChar = (lastSelected != Persona.None) ? lastSelected : DefaultCommander;
            Persona nextChar = character;
            lastSelected = nextChar;
            CommanderChangedEvent?.Invoke(prevChar, nextChar);
        }

        /// <summary>
        /// Subscribe to a commander change event.
        /// </summary>
        /// <param name="listener">The listener to activate when a commander changes</param>
        /// <returns>The default first commander on level startup.</returns>
        public Persona SubscribeToCommanderChange(UnityAction<Persona, Persona> listener) {
            CommanderChangedEvent += listener;
            return DefaultCommander;
        }

        private IEnumerator RunChangeCooldown() {
            yield return new WaitForSeconds(CHANGE_COOLDOWN);
            changeable = true;
        }

        /// <inheritdoc/>
        protected override IEnumerator Switch(bool switchIn, float time, UnityAction callback) {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(base.Switch(switchIn, time, callback));

            time = Mathf.Max(0, (time == -1) ? defaultFadeTime : time);
            Vector3 from = transform.localScale;
            Vector3 to = switchIn ? Vector3.one : Vector3.zero;
            float timer = 0;

            while (timer <= time) {
                timer += Time.deltaTime;
                transform.localScale = Vector3.Lerp(from, to, timer / time);
                yield return null;
            }
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