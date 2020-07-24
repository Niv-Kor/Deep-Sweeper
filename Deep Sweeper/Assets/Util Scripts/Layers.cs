using UnityEngine;

namespace Constants
{
    public static class Layers
    {
        //names
        public static readonly string NAME_DEFAULT = "Default";
        public static readonly string NAME_TRANSPARENT_FX = "TransparentFX";
        public static readonly string NAME_IGNORE_RAYCAST = "Ignore Raycast";
        public static readonly string NAME_WATER = "Water";
        public static readonly string NAME_WATER_BOUNDS = "Water Bounds";
        public static readonly string NAME_PLANT = "Plant";
        public static readonly string NAME_UI = "UI";
        public static readonly string NAME_GROUND = "Ground";
        public static readonly string NAME_PLAYER = "Player";
        public static readonly string NAME_CAMERA = "Camera";
        public static readonly string NAME_MINIMAP = "Minimap";

        //masks
        public static readonly LayerMask DEFAULT = LayerMask.GetMask(NAME_DEFAULT);
        public static readonly LayerMask TRANSPARENT_FX = LayerMask.GetMask(NAME_TRANSPARENT_FX);
        public static readonly LayerMask IGNORE_RAYCAST = LayerMask.GetMask(NAME_IGNORE_RAYCAST);
        public static readonly LayerMask WATER = LayerMask.GetMask(NAME_WATER);
        public static readonly LayerMask WATER_BOUNDS = LayerMask.GetMask(NAME_WATER_BOUNDS);
        public static readonly LayerMask PLANT = LayerMask.GetMask(NAME_PLANT);
        public static readonly LayerMask UI = LayerMask.GetMask(NAME_UI);
        public static readonly LayerMask GROUND = LayerMask.GetMask(NAME_GROUND);
        public static readonly LayerMask PLAYER = LayerMask.GetMask(NAME_PLAYER);
        public static readonly LayerMask CAMERA = LayerMask.GetMask(NAME_CAMERA);
        public static readonly LayerMask MINIMAP = LayerMask.GetMask(NAME_MINIMAP);

        /// <summary>
        /// Check if a certain layer is contained in a layer mask.
        /// </summary>
        /// <param name="layer">The layer to check (as is 'gameObject.layer')</param>
        /// <param name="mask">A mask that may contain the layer</param>
        /// <returns>True if the mask contains the layer.</returns>
        public static bool ContainedInMask(int layer, LayerMask mask) {
            return (mask & 1 << layer) == 1 << layer;
        }
    }
}