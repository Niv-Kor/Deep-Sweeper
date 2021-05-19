using DeepSweeper.Characters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Gameplay.UI.Diegetics.Commander
{
    public class CommanderPanel : Singleton<CommanderPanel>
    {
        #region Exposed Editor Parameter
        [Header("Prefabs")]
        [Tooltip("The commander thumbnail prefabs")]
        [SerializeField] private CommanderThumbnail thumbnailPrefab;

        [Tooltip("A list of available playable commanders.")]
        [SerializeField] private List<CommanderThumbnailConfig> commandersConfig;

        [Header("Placement")]
        [Tooltip("The position of first thumbnail in line from the left.")]
        [SerializeField] private Vector3 firstPosition;

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
        private List<CommanderThumbnail> commanders;
        private CommanderThumbnail selectedCommander;
        private int defaultCommenaderIndex;
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
            this.commanders = new List<CommanderThumbnail>();
        }

        private void Start() {
            RectTransform thumbnailRect = thumbnailPrefab.GetComponent<RectTransform>();
            this.thumbnailDim = thumbnailRect.sizeDelta;
            this.defaultCommenaderIndex = 0;
            this.changeable = true;
            CreateThumbnails();
            OnCommanderSelect(defaultCommenaderIndex);
            this.initialized = true;

            //bind events
            PlayerController.Instance.CommanderSelectionEvent += OnCommanderSelect;
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
                CommanderThumbnail instance = Instantiate(thumbnailPrefab);
                int thumbnailIndex = i - noneChar;
                Vector3 position = firstPosition + Vector3.right * unitWidth * thumbnailIndex;
                instance.transform.SetParent(transform);
                instance.transform.localPosition = position;
                instance.transform.localScale = Vector3.one;
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
            if (!changeable || index >= commanders.Count || index < 0) return;

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
        public CommanderThumbnail GetCommander(CharacterPersona character, int index = 0) {
            List<CommanderThumbnail> list = (from commander in commanders
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
    }
}