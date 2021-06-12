using DeepSweeper.Characters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static DeepSweeper.UI.Ingame.Spatials.Commander.RadialToolkit;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class SectorialDivisor : MonoBehaviour
    {
        private struct CompatibleSegment
        {
            public Segment Segment;
            public int Priority;
        }

        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The prefab of a sector.")]
        [SerializeField] private SectorManager sectorPrefab;
        #endregion

        #region Events
        /// <param type=typeof(SectorManager)>The selected sector</param>
        public event UnityAction<SectorManager> SectorSelectedEvent;
        #endregion

        #region Properties
        public RadialDivision Division { get; private set; }
        public SectorManager CurrentSector { get; private set; }
        public List<SectorManager> Sectors { get; private set; }
        #endregion

        private void Awake() {
            this.Sectors = new List<SectorManager>();
        }

        /// <summary>
        /// Check if a movement vector (representing a mouse movement) from a given
        /// starting position, to a given segment, actually meets the conditions required
        /// to move to that segment and select it.
        /// </summary>
        /// <param name="pos">The starting position vector [([-1:1], [-1:1])]</param>
        /// <param name="segment">The target segment to check</param>
        /// <param name="delta">The movement vector of the mouse</param>
        /// <param name="threshold">
        /// The threshold movement sensitivity.
        /// If the movement vector does not meet this threshold,
        /// it will not pass the nevigation test.
        /// </param>
        /// <param name="conditionsPassed">
        /// If the nevigation test is successful, this parameter
        /// will return the amount of axis that actually passed it organically,
        /// as oppose to the ones that only passed it technically (when the test
        /// subject value is 0, thus every value will pass it).
        /// 
        /// This parameter is used to determine which nevigation tests
        /// had passed more successfully than others, and therefore helps
        /// constructing a priority structure for the tested segments.
        /// </param>
        /// <returns>True if the specified segment has passed the test and can be nevigated to.</returns>
        private bool CheckRelativeNevigation(Vector2 pos, Segment segment, Vector2 delta, float threshold, out int conditionsPassed) {
            //tailor a customized condition for the current position
            Vector2 target = segment.AsCoordinates();
            conditionsPassed = 0;

            //only compare the dominant axles
            if (pos.x == 0 && target.x != 0) pos.y = target.y; //ignore Y axis
            if (pos.y == 0 && target.y != 0) pos.x = target.x; //ignore X axis
            if (target.x == 0) target.x = pos.x; //ingore X axis
            if (target.y == 0) target.y = pos.y; //ignore Y axis

            Vector2 diff = (target - pos).normalized;
            Vector2 condition = diff * threshold;

            if (condition != Vector2.zero) {
                bool passed = delta.Greater(condition, out bool[] detailedTest);

                //count the amount of passed conditions
                if (passed) {
                    foreach (bool test in detailedTest)
                        if (test) conditionsPassed++;
                }

                return passed;
            }
            else return false;
        }

        /// <summary>
        /// Temporarily select a sector.
        /// </summary>
        /// <param name="sector">The sector to select</param>
        /// <returns>True if the sector has been successfully selected.</returns>
        private bool SelectSector(SectorManager sector) {
            if (sector is null || sector == CurrentSector) return false;

            if (sector.IsAvailable) {
                sector.Selected = true;
                if (CurrentSector != null) CurrentSector.Selected = false;
                CurrentSector = sector;
                SectorSelectedEvent?.Invoke(sector);
                return true;
            }
            else return false;
        }

        /// <see cref="SelectSector(SectorManager)"/>
        /// <param name="segment">The segment to select the sector of whom</param>
        private bool SelectSector(Segment segment) {
            SectorManager sector = Sectors?.Find(x => x.Segment == segment);
            return SelectSector(sector);
        }

        /// <see cref="SelectSector(SectorManager)"/>
        /// <param name="segment">The character to select the sector of whom</param>
        public bool SelectSector(Persona character) {
            SectorManager sector = Sectors?.Find(x => x.Character == character);
            return SelectSector(sector);
        }

        /// <param name="character">A character to get the sector of whom</param>
        /// <returns>The character's sector.</returns>
        public SectorManager GetSector(Persona character) {
            return Sectors?.Find(x => x.Character == character);
        }

        /// <summary>
        /// Divide the circle to sectors, containing each of the characters in a given list.
        /// </summary>
        /// <param name="characters">A list of characters</param>
        public void PopulateCharacters(List<Persona> characters) {
            //populate counter clockwise
            List<Persona> cloneList = new List<Persona>(characters);
            cloneList.Reverse();

            int amount = cloneList.Count;
            Division = Divide(amount);
            List<Segment> segments = Division.AsSegments();

            //segmentate
            for (int i = 0; i < segments.Count; i++) {
                Segment segment = segments[i];
                Persona character = cloneList[i];
                SectorManager instance = Instantiate(sectorPrefab);

                instance.name = segment.ToString();
                instance.transform.SetParent(transform);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localScale = Vector3.one;
                instance.Character = character;
                instance.Build(segment, character);
                Sectors.Add(instance);
            }
        }

        /// <summary>
        /// Nevigate from one sector to another based on the movement of the mouse.
        /// </summary>
        /// <param name="delta">The movement vector of the mouse</param>
        /// <param name="threshold">
        /// The threshold movement sensitivity.
        /// If the movement vector does not meet this threshold,
        /// it will not pass the nevigation test to any segment.
        /// </param>
        /// <returns>True if a nevigation to another segment came out successful.</returns>
        public bool NavigateToSector(Vector2 delta, float threshold) {
            if (Mathf.Abs(delta.x) < threshold && Mathf.Abs(delta.y) < threshold) return false;

            Segment selected = Segment.Sector0_0;
            List<Segment> segments = Division.AsSegments();
            Vector2 currentPos = (CurrentSector is null) ? Vector2.zero : CurrentSector.Segment.AsCoordinates();
            List<CompatibleSegment> compatibles = new List<CompatibleSegment>();

            //check agains each segment of the division
            foreach (Segment segment in segments) {
                if (CurrentSector != null && segment == CurrentSector.Segment) continue;

                //found this segment compatible
                if (CheckRelativeNevigation(currentPos, segment, delta, threshold, out int conditionsPassed)) {
                    CompatibleSegment compatible;
                    compatible.Segment = segment;
                    compatible.Priority = conditionsPassed;
                    compatibles.Add(compatible);

                    //best match for a 2D vector comparison
                    if (compatible.Priority == 2) break;
                }
            }

            //find the best match of the compatible segments
            int maxPriority = 0;
            foreach (CompatibleSegment compatible in compatibles) {
                if (compatible.Priority > maxPriority) {
                    maxPriority = compatible.Priority;
                    selected = compatible.Segment;
                }
            }

            return selected != Segment.Sector0_0 && SelectSector(selected);
        }
    }
}