using DeepSweeper.Characters;
using System.Collections.Generic;
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
        [Tooltip("The default cooldown time between each two commanders (in seconds).")]
        [SerializeField] private int defaultCooldown;
        #endregion

        #region Class Members
        private Vector2 thumbnailDim;
        private List<CommanderThumbnail> commanders;
        private CommanderThumbnail selectedCommander;
        private bool initialized;
        #endregion

        #region Events
        /// <param type=typeof(CharacterPersona)>The changed character</param>
        /// <param type=typeof(CharacterPersona)>The new character</param>
        public event UnityAction<CharacterPersona, CharacterPersona> CommanderChangedEvent;
        #endregion

        private void Awake() {
            this.commanders = new List<CommanderThumbnail>();
        }

        private void Start() {
            RectTransform thumbnailRect = thumbnailPrefab.GetComponent<RectTransform>();
            this.thumbnailDim = thumbnailRect.sizeDelta;
            CreateThumbnails();
            OnCommanderSelect(0);
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
        /// Select a single commander and deselect the rest.
        /// </summary>
        /// <param name="index">Index of the commander to select</param>
        private void OnCommanderSelect(int index) {
            if (index >= commanders.Count || index < 0) return;

            //on cooldown
            if (commanders[index].OnCooldown) {
                commanders[index].ShowForbiddenSign();
                return;
            }

            //find changed commander
            CharacterPersona prevCommander = initialized ? selectedCommander.Character : CharacterPersona.None;

            foreach (var commander in commanders) {
                bool selected = commanders.IndexOf(commander) == index;
                commander.Select(selected, !initialized);

                if (selected) selectedCommander = commander;
            }

            CommanderChangedEvent?.Invoke(prevCommander, selectedCommander.Character);
        }
    }
}