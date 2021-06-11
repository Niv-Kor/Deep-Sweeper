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
        [SerializeField] private SectorManager sectorPrefab;

        [SerializeField] private float sensitivity = 1;
        #endregion

        #region Class Members
        private RadialDivision division;
        private List<SectorManager> sectors;

        private Vector2 lastDelta;
        #endregion

        #region Events
        public event UnityAction<SectorManager> SectorSelectedEvent;
        #endregion

        #region Properties
        public SectorManager CurrentSector { get; private set; }
        #endregion

        private void Awake() {
            this.sectors = new List<SectorManager>();
        }

        private bool CheckRelativeNevigation(Segment segment, Vector2 pos, Vector2 delta, out int conditionsPassed) {
            //tailor a customized condition for the current position
            Vector2 target = segment.AsCoordinates();
            conditionsPassed = 0;

            //only compare the dominant axles
            if (pos.x == 0 && target.x != 0) pos.y = target.y; //ignore Y axis
            if (pos.y == 0 && target.y != 0) pos.x = target.x; //ignore X axis
            if (target.x == 0) target.x = pos.x; //ingore X axis
            if (target.y == 0) target.y = pos.y; //ignore Y axis

            Vector2 diff = (target - pos).normalized;
            Vector2 condition = diff * sensitivity;

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
        /// Select a character sector.
        /// </summary>
        /// <param name="division">The radial division of the characters</param>
        /// <param name="segment">The segment of the selected sector</param>
        private bool SelectSector(SectorManager sector) {
            if (sector is null || sector == CurrentSector) return false;

            if (CurrentSector != null) CurrentSector.Selected = false;
            sector.Selected = true;
            CurrentSector = sector;
            SectorSelectedEvent?.Invoke(sector);
            return true;
        }

        /// <summary>
        /// Select a character sector.
        /// </summary>
        /// <param name="division">The radial division of the characters</param>
        /// <param name="segment">The segment of the selected sector</param>
        private bool SelectSector(Segment segment) {
            SectorManager sector = sectors?.Find(x => x.Segment == segment);
            return SelectSector(sector);
        }

        public void SelectSector(Persona character) {
            SectorManager sector = sectors?.Find(x => x.Character == character);
            SelectSector(sector);
        }

        /// <summary>
        /// Create the radial sectors and set a character sprite in each one.
        /// </summary>
        /// <param name="characters">A list of characters</param>
        public void PopulateCharacters(List<Persona> characters) {
            //populate counter clockwise
            List<Persona> cloneList = new List<Persona>(characters);
            cloneList.Reverse();

            int amount = cloneList.Count;
            division = Divide(amount);
            List<Segment> segments = division.AsSegments();

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
                instance.Build(segment);
                sectors.Add(instance);
            }
        }

        public bool NavigateToSector(Vector2 delta) {
            if (lastDelta != Vector2.zero && delta.Greater(lastDelta)) return false;

            //delta is strong enough
            if (Mathf.Abs(delta.x) >= sensitivity || Mathf.Abs(delta.y) >= sensitivity) {
                Segment selected = Segment.Sector0_0;
                List<Segment> segments = division.AsSegments();
                Vector2 currentPos = (CurrentSector is null) ? Vector2.zero : CurrentSector.Segment.AsCoordinates();
                List<CompatibleSegment> compatibles = new List<CompatibleSegment>();

                //check agains each segment of the division
                foreach (Segment segment in segments) {
                    if (CurrentSector != null && segment == CurrentSector.Segment) continue;

                    //found this segment compatible
                    if (CheckRelativeNevigation(segment, currentPos, delta, out int conditionsPassed)) {
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

                lastDelta = delta;
                return selected != Segment.Sector0_0 && SelectSector(selected);
            }
            else return false;
        }
    }
}