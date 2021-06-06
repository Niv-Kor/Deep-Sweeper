using DeepSweeper.Characters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class SectorialDivisor : MonoBehaviour
    {
        private enum RadialDivision
        {
            None,
            Single,
            Double,
            Triple,
            Quadraple,
        }

        [Serializable]
        private struct SectorStructure
        {
            [Tooltip("The type of characters division.")]
            [SerializeField] public RadialDivision Division;

            [Tooltip("A list of sector prefabs to match the division.\n"
                   + "The sectors must be divided from the lowest angle to the highest.")]
            [SerializeField] public List<SectorManager> Sectors;
        }

        #region Exposed Editor Parameters
        [Tooltip("A list of defined sector structures.")]
        [SerializeField] private List<SectorStructure> sectorStructures;
        #endregion

        /// <param name="amount">Amount of characters that the circle should contain.</param>
        /// <returns>
        /// the correct radial division type for a given amount of characters.
        /// If the amount of characters is not between 1-4 (inclusive),
        /// this method would return the type 'None'.
        /// </returns>
        private static RadialDivision GetDivision(int amount) {
            switch (amount) {
                case 1: return RadialDivision.Single;
                case 2: return RadialDivision.Double;
                case 3: return RadialDivision.Triple;
                case 4: return RadialDivision.Quadraple;
                default: return RadialDivision.None;
            }
        }

        /// <summary>
        /// Create the radial sectors and set a character sprite in each one.
        /// </summary>
        /// <param name="characters">A list of characters</param>
        public void PopulateCharacters(List<CharacterPersona> characters) {
            //populate characters counter clockwise
            characters.Reverse();

            int amount = characters.Count;
            SectorStructure structure = sectorStructures.Find(x => x.Division == GetDivision(amount));

            //found a fitting structure
            if (structure.Sectors != null) {
                var sectors = structure.Sectors;

                //instantiate sectors and populate them with characters
                for (int i = 0; i < sectors.Count; i++) {
                    CharacterPersona character = characters[i];
                    SectorManager prefab = sectors[i];
                    SectorManager instance = Instantiate(prefab);

                    instance.transform.SetParent(transform);
                    instance.transform.localPosition = Vector3.zero;
                    instance.Character = character;
                }
            }
        }
    }
}