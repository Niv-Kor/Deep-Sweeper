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
        [Header("Settings")]
        [Tooltip("A list of characters to be available for selection during this level.")]
        [SerializeField] private List<Persona> Characters;

        [Tooltip("The force the player needs to apply to navigate between different sectors.")]
        [SerializeField] private float sensitivity = 1;
        #endregion

        #region Constants
        private static readonly float CHANGE_COOLDOWN = .1f;
        private static readonly string NO_CHARACTERS_ERROR = "No characters available for selection.";
        #endregion

        #region Class Members
        private SectorManager lastSelectedSector;
        private SectorialDivisor sectorialDivisor;
        private PlayerController controls;
        private Coroutine cooldownCoroutine;
        private Coroutine fadeCoroutine;
        private Delimeter delimeter;
        private Persona lastSelectedCharacter;
        private bool changeable;
        #endregion

        #region Events
        /// <param type=typeof(CharacterPersona)>The changed character</param>
        /// <param type=typeof(CharacterPersona)>The new character</param>
        public event UnityAction<Persona, Persona> CommanderChangedEvent;

        /// <param type=typeof(Persona)>The dead commander</param>
        /// <param type=typeof(List<Persona>)>A list of alive alternative commanders</param>
        public event UnityAction<Persona, List<Persona>> CommanderDeadEvent;
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

        private void OnValidate() {
            if (Application.isPlaying) {
                //remove duplicate commanders
                ISet<Persona> set = new HashSet<Persona>();

                for (int i = 0; i < Characters.Count; i++) {
                    Persona character = Characters[i];

                    if (!set.Contains(character)) set.Add(character);
                    else Characters.RemoveAt(i);
                }

                if (Characters.Count == 0) throw new System.Exception(NO_CHARACTERS_ERROR);
            }
        }

        /// <summary>
        /// Activate when the player presses the selection key.
        /// This method enables the selection of a commander.
        /// </summary>
        private void OnSelectionKeyDown() {
            void EnableSelection() { controls.MouseMoveEvent += OnMouseMovement; }
            Activate(true, -1, EnableSelection);
        }

        /// <summary>
        /// Activate when the player releases the selection key.
        /// This method disables the selection of a commander.
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
            if (!changeable || !sectorialDivisor.NavigateToSector(delta, sensitivity)) return;

            changeable = false;
            if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = StartCoroutine(RunChangeCooldown());
        }

        /// <summary>
        /// Select the character from the currently highlighted sector.
        /// </summary>
        /// <returns>True if a character has been successfully selected.</returns>
        private bool SelectCharacter() {
            SectorManager current = sectorialDivisor.CurrentSector;
            bool success = current != null && current != lastSelectedSector;
            return success && SelectCharacter(current.Character);
        }

        /// <summary>
        /// Select a specific character.
        /// This method does nothing if there isn't a sector
        /// that consists of the given character.
        /// </summary>
        /// <param name="character">The character to select</param>
        /// <returns>True if a character has been successfully selected.</returns>
        private bool SelectCharacter(Persona character) {
            if (sectorialDivisor.CurrentSector?.Character != character)
                sectorialDivisor.SelectSector(character);

            Persona prevChar = lastSelectedCharacter;
            Persona nextChar = character;
            if (LevelFlow.Instance.DuringPhase) lastSelectedSector?.Cooldown.Begin();

            lastSelectedCharacter = nextChar;
            lastSelectedSector = sectorialDivisor.GetSector(lastSelectedCharacter);
            CommanderChangedEvent?.Invoke(prevChar, nextChar);
            return true;
        }

        /// <summary>
        /// Announce the death or a resurrection of a character.
        /// This method makes the specified character unusable if it's killed,
        /// or usable again if it's ressurected.
        /// </summary>
        /// <param name="character">The character to kill or ressurect</param>
        /// <param name="flag">True to kill the character or false to ressurect it</param>
        /// <seealso cref="CommanderDeadEvent"/>
        public void AnnounceDeath(Persona character, bool flag) {
            SectorManager sector = sectorialDivisor.GetSector(character);
            bool success = sector != null && (flag ? sector.Kill() : sector.Resurrect());

            if (success) {
                //construct a list of all remaining commanders
                List<Persona> alive = new List<Persona>();

                foreach (Persona commander in Characters) {
                    SectorManager commanderSector = sectorialDivisor.GetSector(commander);

                    if (commanderSector != null && !commanderSector.IsDead)
                        alive.Add(commander);
                }

                CommanderDeadEvent?.Invoke(character, alive);
            }
        }

        /// <summary>
        /// Subscribe to a commander change event.
        /// </summary>
        /// <param name="listener">The listener to activate when a commander changes</param>
        /// <seealso cref="CommanderChangedEvent"/>
        /// <returns>The default first commander on level startup.</returns>
        public Persona SubscribeToCommanderChange(UnityAction<Persona, Persona> listener) {
            CommanderChangedEvent += listener;
            return DefaultCommander;
        }

        /// <summary>
        /// Run the cooldown of the flag that prevents the
        /// player from selecting a different sector too quickly.
        /// Another sector can be selected once again after the cooldown is over.
        /// </summary>
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
        public override void OnPhasePauses(Phase phase) {
            foreach (SectorManager sector in sectorialDivisor.Sectors)
                sector.Cooldown.Pause();

            Activate(false);
        }

        /// <inheritdoc/>
        public override void OnPhaseResumes(Phase phase) {
            foreach (SectorManager sector in sectorialDivisor.Sectors)
                sector.Cooldown.Resume();
        }

        /// <inheritdoc/>
        public override void OnPhaseEnds(Phase phase, bool success) { Activate(false); }
    }
}