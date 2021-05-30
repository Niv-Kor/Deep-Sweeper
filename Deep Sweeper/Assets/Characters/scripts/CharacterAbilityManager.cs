using DeepSweeper.Gameplay.UI.Diegetics.Commander;
using DeepSweeper.UI.Ingame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeepSweeper.Characters
{
    public abstract class CharacterAbilityManager<T, A> : Singleton<T> where T : MonoBehaviour where A : IAbilityModel
    {
        #region Exposed Editor Parameters
        [Tooltip("A list of ability configurations for the available pool of characters.")]
        [SerializeField] protected List<CharacterAbilityConfig<A>> abilities;
        #endregion

        #region Properties
        protected CharacterAbilityConfig<A> CurrentConfig { get; set; }
        #endregion

        protected virtual void Start() {
            CommanderDiegetic commander = DiegeticsManager.Instance.Get(typeof(CommanderDiegetic)) as CommanderDiegetic;
            var firstCommander = commander.SubscribeToCommanderChange(OnChangeCommander);
            OnChangeCommander(CharacterPersona.None, firstCommander);
        }

        /// <summary>
        /// Activate when the commander changes.
        /// </summary>
        /// <param name="prev">Old commander character</param>
        /// <param name="next">New commander character</param>
        protected virtual void OnChangeCommander(CharacterPersona prev, CharacterPersona next) {
            if (next == prev) return;

            bool newAvailable = GetAbilityConfig(next, out CharacterAbilityConfig<A> newConfig);

            if (newAvailable) {
                //remove old if exists
                bool oldAvailable = GetAbilityConfig(prev, out CharacterAbilityConfig<A> oldConfig);
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
        protected bool GetAbilityConfig(CharacterPersona character, out CharacterAbilityConfig<A> config) {
            if (abilities is null) {
                config = default;
                return false;
            }

            //find character's ability
            var list = (from ability in abilities
                        where ability.Character == character
                        select ability).ToList();

            bool available = list.Count > 0;
            config = available ? list[0] : default;
            return available;
        }

        /// <summary>
        /// Unlink the previous abilites.
        /// </summary>
        /// <param name="abilities">The changed ability</param>
        protected abstract void StripAbility(A ability);

        /// <summary>
        /// Apply the new abilites.
        /// </summary>
        /// <param name="ability">The new applied ability</param>
        protected abstract void ApplyAbility(A ability);
    }
}