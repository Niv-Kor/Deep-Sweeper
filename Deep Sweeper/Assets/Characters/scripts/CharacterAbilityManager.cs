using DeepSweeper.Gameplay.UI.Diegetics.Commander;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeepSweeper.Characters
{
    public abstract class CharacterAbilityManager<T> : MonoBehaviour where T : System.Enum
    {
        [System.Serializable]
        protected struct CharacterAbilityConfig
        {
            [Tooltip("The character that possesses the ability.")]
            public CharacterPersona Character;

            [Tooltip("Special ability of the specified character.")]
            public T Ability;
        }

        #region Exposed Editor Parameters
        [Tooltip("A list of ability configurations for the available pool of characters.")]
        [SerializeField] protected List<CharacterAbilityConfig> abilities;
        #endregion

        #region Properties
        protected CharacterAbilityConfig CurrentConfig { get; set; }
        #endregion

        protected virtual void Awake() {
            CommanderPanel.Instance.CommanderChangedEvent += OnChangeCommander;
        }

        /// <summary>
        /// Activate when the commander changes.
        /// </summary>
        /// <param name="prev">Old commander character</param>
        /// <param name="next">New commander character</param>
        protected virtual void OnChangeCommander(CharacterPersona prev, CharacterPersona next) {
            bool newAvailable = GetAbilityConfig(next, out CharacterAbilityConfig newConfig);

            if (newAvailable) {
                //remove old if exists
                bool oldAvailable = GetAbilityConfig(prev, out CharacterAbilityConfig oldConfig);
                if (oldAvailable) StripAbility(oldConfig.Ability);

                //apply new
                CurrentConfig = newConfig;
                ApplyAbility(newConfig.Ability);
            }
        }

        /// <summary>
        /// Get the ability configuration of a specific character.
        /// </summary>
        /// <param name="character">The character to get the ability configuration of which</param>
        /// <param name="ability">The output configuration</param>
        /// <returns>
        /// True if the ability configuration is available.
        /// If false, the ability returned is the default struct value.
        /// </returns>
        protected bool GetAbilityConfig(CharacterPersona character, out CharacterAbilityConfig config) {
            List<CharacterAbilityConfig> list = (from ability in abilities
                                                 where ability.Character == character
                                                 select ability).ToList();

            bool available = list.Count > 0;
            config = available ? list[0] : default;
            return available;
        }

        /// <summary>
        /// Unlink the previous ability.
        /// </summary>
        /// <param name="ability">The changed ability type</param>
        protected abstract void StripAbility(T ability);

        /// <summary>
        /// Apply the new ability.
        /// </summary>
        /// <param name="ability">The new ability type</param>
        protected abstract void ApplyAbility(T ability);
    }
}