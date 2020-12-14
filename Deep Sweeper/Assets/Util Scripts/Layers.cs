using UnityEngine;

namespace Constants
{
    public static class Layers
    {
        //names
        private static readonly string NAME_DEFAULT = "Default";
        private static readonly string NAME_TRANSPARENT_FX = "TransparentFX";
        private static readonly string NAME_IGNORE_RAYCAST = "Ignore Raycast";
        private static readonly string NAME_WATER = "Water";
        private static readonly string NAME_UI = "UI";
        private static readonly string NAME_PARTICLES = "Particles";
        private static readonly string NAME_GROUND = "Ground";
        private static readonly string NAME_PLAYER = "Player";
        private static readonly string NAME_CREATURE = "Creature";
        private static readonly string NAME_FISH_BORDERS = "Fish Borders";
        private static readonly string NAME_BULLET = "Bullet";
        private static readonly string NAME_MINE = "Mine";
        private static readonly string NAME_MINE_INDICATION = "Mine Indication";
        private static readonly string NAME_FLAGGED_MINE = "Flagged Mine";
        private static readonly string NAME_MINIMAP = "Minimap";
        private static readonly string NAME_GATE = "Gate";

        //masks
        public static readonly LayerMask DEFAULT = LayerMask.GetMask(NAME_DEFAULT);
        public static readonly LayerMask TRANSPARENT_FX = LayerMask.GetMask(NAME_TRANSPARENT_FX);
        public static readonly LayerMask IGNORE_RAYCAST = LayerMask.GetMask(NAME_IGNORE_RAYCAST);
        public static readonly LayerMask WATER = LayerMask.GetMask(NAME_WATER);
        public static readonly LayerMask UI = LayerMask.GetMask(NAME_UI);
        public static readonly LayerMask PARTICLES = LayerMask.GetMask(NAME_PARTICLES);
        public static readonly LayerMask GROUND = LayerMask.GetMask(NAME_GROUND);
        public static readonly LayerMask PLAYER = LayerMask.GetMask(NAME_PLAYER);
        public static readonly LayerMask CREATURE = LayerMask.GetMask(NAME_CREATURE);
        public static readonly LayerMask FISH_BORDERS = LayerMask.GetMask(NAME_FISH_BORDERS);
        public static readonly LayerMask BULLET = LayerMask.GetMask(NAME_BULLET);
        public static readonly LayerMask MINE = LayerMask.GetMask(NAME_MINE);
        public static readonly LayerMask MINE_INDICATION = LayerMask.GetMask(NAME_MINE_INDICATION);
        public static readonly LayerMask FLAGGED_MINE = LayerMask.GetMask(NAME_FLAGGED_MINE);
        public static readonly LayerMask MINIMAP = LayerMask.GetMask(NAME_MINIMAP);
        public static readonly LayerMask GATE = LayerMask.GetMask(NAME_GATE);

        /// <summary>
        /// Check if a certain layer is contained in a layer mask.
        /// </summary>
        /// <param name="layer">The layer to check (as is 'gameObject.layer')</param>
        /// <param name="mask">A mask that may contain the layer</param>
        /// <returns>True if the mask contains the layer.</returns>
        public static bool ContainedInMask(int layer, LayerMask mask) {
            return (mask & 1 << layer) == 1 << layer;
        }

        /// <summary>
        /// Get the layer value of a layer mask.
        /// This function only works well with one layered masks.
        /// </summary>
        /// <param name="mask">The mask from which to extract the value</param>
        /// <returns>
        /// The value of the layer mask,
        /// or the last one if it contains multiple layers.
        /// </returns>
        public static int GetLayerValue(LayerMask mask) {
            return (int) Mathf.Log(mask.value, 2);
        }
    }
}