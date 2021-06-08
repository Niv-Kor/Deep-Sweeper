using DeepSweeper.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class SectorialDivisor : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [SerializeField] private SectorManager sectorPrefab;

        [SerializeField] private float sensitivity = 1;
        #endregion

        #region Class Members
        private RadialToolkit.RadialDivision division;
        private List<SectorManager> sectors;
        #endregion

        #region Properties
        public SectorManager CurrentSector { get; private set; }
        #endregion

        private void Segmentate(RadialToolkit.RadialDivision division) {
            List<RadialToolkit.Segment> segments = division.AsSegments();
            sectors = new List<SectorManager>();
            
            //build each segment
            foreach (RadialToolkit.Segment segment in segments) {
                SectorManager instance = Instantiate(sectorPrefab);
                instance.name = segment.ToString();
                instance.transform.SetParent(transform);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localScale = Vector3.one;
                instance.Build(segment);
                sectors.Add(instance);
            }
        }

        /// <summary>
        /// Select a character sector.
        /// </summary>
        /// <param name="division">The radial division of the characters</param>
        /// <param name="segment">The segment of the selected sector</param>
        private bool SelectSector(RadialToolkit.Segment segment) {
            SectorManager sector = sectors?.Find(x => x.Segment == segment);
            if (sector is null || sector == CurrentSector) return false;

            if (CurrentSector != null) CurrentSector.Selected = false;
            sector.Selected = true;
            CurrentSector = sector;
            return true;
        }

        /// <summary>
        /// Create the radial sectors and set a character sprite in each one.
        /// </summary>
        /// <param name="characters">A list of characters</param>
        public void PopulateCharacters(List<CharacterPersona> characters) {
            //populate counter clockwise
            characters.Reverse();

            int amount = characters.Count;
            division = RadialToolkit.Divide(amount);
            Segmentate(division);
        }

        public bool NavigateToSector(Vector2 delta) {
            print("navigate to " + delta);

            RadialToolkit.Segment segment = default;
            float e = sensitivity;
            float x = delta.x;
            float y = delta.y;

            switch (division) {
                case RadialToolkit.RadialDivision.Double:
                    if (x > e) segment = RadialToolkit.Segment.Sector0_180;
                    else if (x < -e) segment = RadialToolkit.Segment.Sector180_360;
                    break;

                case RadialToolkit.RadialDivision.Triple:
                    if (x > e && y >= 0) segment = RadialToolkit.Segment.Sector0_120;
                    else if (y < -e) segment = RadialToolkit.Segment.Sector120_240;
                    else if (x < -e && y >= 0) segment = RadialToolkit.Segment.Sector240_360;
                    break;

                case RadialToolkit.RadialDivision.Quadruple:
                    if (x > e && y > e / 2) segment = RadialToolkit.Segment.Sector0_90;
                    else if (x > e && y < -e / 2) segment = RadialToolkit.Segment.Sector90_180;
                    else if (x < -e && y < -e / 2) segment = RadialToolkit.Segment.Sector180_270;
                    else if (x < -e && y > e / 2) segment = RadialToolkit.Segment.Sector270_360;
                    break;

                default: return false;
            }

            return SelectSector(segment);
        }
    }
}