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
        private static readonly string NAME_INACTIVE_MINE = "Inactive Mine";
        private static readonly string NAME_DEPTH_UI = "Depth UI";
        private static readonly string NAME_SANDBOX = "Sandbox";
        private static readonly string NAME_TARGET_MINE = "Target Mine";

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
        public static readonly LayerMask INACTIVE_MINE = LayerMask.GetMask(NAME_INACTIVE_MINE);
        public static readonly LayerMask DEPTH_UI = LayerMask.GetMask(NAME_DEPTH_UI);
        public static readonly LayerMask SANDBOX = LayerMask.GetMask(NAME_SANDBOX);
        public static readonly LayerMask TARGET_MINE = LayerMask.GetMask(NAME_TARGET_MINE);

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

        /// <summary>
        /// Apply a layer to a game object.
        /// </summary>
        /// <param name="obj">The object to which to apply the layer</param>
        /// <param name="layer">The applied layer</param>
        /// <param name="applyToChildren">True to also apply the layer to each of the object's children</param>
        public static void ApplyLayer(GameObject obj, LayerMask layer, bool applyToChildren = false) {
            int layerVal = GetLayerValue(layer);
            obj.layer = layerVal;

            if (applyToChildren)
                foreach (Transform child in obj.transform)
                    child.gameObject.layer = layerVal;
        }
    }
}