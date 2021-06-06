using DeepSweeper.Characters;
using DeepSweeper.Flow;
using DeepSweeper.Player.Controls;
using DeepSweeper.UI.Ingame;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Gameplay.UI.Diegetics.Commander
{
    public class CommanderDiegetic : Diegetic
    {
        #region Exposed Editor Parameter
        [Header("Prefabs")]
        [Tooltip("The commander thumbnail prefabs")]
        [SerializeField] private Thumbnail thumbnailPrefab;

        [Tooltip("A list of available playable commanders.")]
        [SerializeField] private List<ThumbnailModel> commandersConfig;

        [Header("Placement")]
        [Tooltip("The position of first thumbnail in line from the left.")]
        [SerializeField] private Vector2 firstPosition;

        [Tooltip("The space between each two neighbour thumbnails.")]
        [SerializeField] private float space;

        [Header("Timing")]
        [Tooltip("The time the user has to wait before changin a commander (in seconds).\n"
               + "This parameter is critical for preventing bugs that might occur when the"
               + "player changes a commander too recently.")]
        [SerializeField] private float mandatoryCooldown = .1f;

        [Tooltip("The default cooldown time between each two commanders (in seconds).")]
        [SerializeField] private int defaultCooldown;
        #endregion

        #region Class Members
        private Vector2 thumbnailDim;
        private List<Thumbnail> commanders;
        private Thumbnail selectedCommander;
        private int defaultCommenaderIndex;
        private float thumbnailRadius;
        private bool initialized;
        private bool changeable;
        #endregion

        #region Events
        /// <param type=typeof(CharacterPersona)>The changed character</param>
        /// <param type=typeof(CharacterPersona)>The new character</param>
        public event UnityAction<CharacterPersona, CharacterPersona> CommanderChangedEvent;
        #endregion

        protected override void Awake() {
            base.Awake();
            this.commanders = new List<Thumbnail>();
            this.thumbnailRadius = thumbnailPrefab.GetComponent<RectTransform>().sizeDelta.x;
        }

        protected override void Start() {
            base.Start();

            RectTransform thumbnailRect = thumbnailPrefab.GetComponent<RectTransform>();
            this.thumbnailDim = thumbnailRect.sizeDelta;
            this.defaultCommenaderIndex = 0;
            this.changeable = true;
            CreateThumbnails();
            OnCommanderSelect(defaultCommenaderIndex);
            this.initialized = true;

            //bind events
            //PlayerController.Instance.CommanderSelectionEvent += OnCommanderSelect;
        }

        private void OnValidate() {
            defaultCooldown = Mathf.Max(0, defaultCooldown);
        }

        /// <summary>
        /// Create the thumbnail on the screen,
        /// based on the given configurations list.
        /// </summary>
        private void CreateThumbnails() {
            float unitWidth = thumbnailDim.x + space;

            for (int i = 0, noneChar = 0; i < commandersConfig.Count; i++) {
                var config = commandersConfig[i];

                //no character selected
                if (config.Character == CharacterPersona.None) {
                    noneChar++;
                    continue;
                }

                //instantiate thumbnail
                Thumbnail instance = Instantiate(thumbnailPrefab);
                int thumbnailIndex = i - noneChar;
                Vector2 position = firstPosition + Vector2.right * unitWidth * thumbnailIndex;
                instance.transform.SetParent(transform);
                instance.transform.localPosition = position;
                instance.transform.localScale = Vector2.one;
                instance.SetCommander(config, defaultCooldown);
                commanders.Add(instance);
            }
        }

        /// <summary>
        /// Run the mandatory cooldown between commander changes.
        /// When the timer ends, the player is able to change commanders again.
        /// </summary>
        private IEnumerator RunCooldownTimer() {
            yield return new WaitForSeconds(mandatoryCooldown);
            changeable = true;
        }

        /// <summary>
        /// Select a single commander and deselect the rest.
        /// </summary>
        /// <param name="index">Index of the commander to select</param>
        private void OnCommanderSelect(int index) {
            //changability check
            bool canChange = true;
            canChange &= changeable; //mutable
            canChange &= index < commanders.Count && index >= 0; //within boundaries
            canChange &= commanders.IndexOf(selectedCommander) != index || !initialized; //different commander
            if (!canChange) return;

            //on cooldown
            if (commanders[index].OnCooldown) {
                commanders[index].ShowForbiddenSign();
                return;
            }

            CharacterPersona prevCommander = initialized ? selectedCommander.Character : CharacterPersona.None;
            bool duringPhase = LevelFlow.Instance.DuringPhase;

            foreach (var commander in commanders) {
                bool selected = commanders.IndexOf(commander) == index;

                //allow free switching if the player hadn't started a phase yet
                if (!duringPhase && !selected) commander.SetNextCooldownTime(0);

                commander.Select(selected, !initialized);
                if (selected) selectedCommander = commander;
            }

            CommanderChangedEvent?.Invoke(prevCommander, selectedCommander.Character);
            changeable = false;
            StartCoroutine(RunCooldownTimer());
        }

        /// <summary>
        /// Get a specific commander thumbnail.
        /// </summary>
        /// <param name="character">The commander's character</param>
        /// <param name="index">The index of the commander if there are multiple results (from left)</param>
        /// <returns>The specified commander.</returns>
        public Thumbnail GetCommander(CharacterPersona character, int index = 0) {
            List<Thumbnail> list = (from commander in commanders
                                             where commander.Character == character
                                             select commander).ToList();

            return (list.Count > 0) ? list[index] : null;
        }

        /// <summary>
        /// Subscribe to a commander change event.
        /// </summary>
        /// <param name="listener">The listener to activate when a commander changes</param>
        /// <returns>The default first commander on level startup.</returns>
        public CharacterPersona SubscribeToCommanderChange(UnityAction<CharacterPersona, CharacterPersona> listener) {
            CommanderChangedEvent += listener;
            return commandersConfig[defaultCommenaderIndex].Character;
        }

        /// <inheritdoc/>
        public override void ResetValue(Phase phase) {}

        /// <inheritdoc/>
        public override void OnPhaseStarts(Phase phase) {}

        /// <inheritdoc/>
        public override void OnPhasePauses(Phase phase) { Activate(false); }

        /// <inheritdoc/>
        public override void OnPhaseResumes(Phase phase) { Activate(true); }

        /// <inheritdoc/>
        public override void OnPhaseEnds(Phase phase, bool success) {}
    }
}