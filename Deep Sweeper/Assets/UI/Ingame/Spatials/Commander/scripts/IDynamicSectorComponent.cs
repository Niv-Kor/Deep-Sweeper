using DeepSweeper.Characters;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public interface IDynamicSectorComponent
    {
        /// <summary>
        /// Dynamically build this sector component based on the building
        /// instructions of its segment.
        /// </summary>
        /// <param name="instructions">The building instructions of the segment</param>
        /// <param name="character">The sector's character</param>
        void Build(SegmentInstructions instructions, Persona character = Persona.None);
    }
}