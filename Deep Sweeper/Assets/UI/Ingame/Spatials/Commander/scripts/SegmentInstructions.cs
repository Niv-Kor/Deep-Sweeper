using System;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    [Serializable]
    public struct SegmentInstructions
    {
        [Tooltip("The segment to which these building instructions relate.")]
        [SerializeField] public RadialToolkit.Segment Segment;

        [Tooltip("The direction from which the segment's radial fill starts.")]
        [SerializeField] public Image.Origin360 FillOrigin;

        [Tooltip("True if the radial fill should scale clockwise (false for counter-clockwise).")]
        [SerializeField] public bool Clockwise;

        [Tooltip("A Z axis deviation of the segment.")]
        [SerializeField] public float Roll;

        [Tooltip("A fixed offset for any embedded character sprite in the segment.")]
        [SerializeField] public Vector2 SpriteOffset;

        [Tooltip("The scale of the sprite's mask, relative to the scale of the segment.\n"
               + "The mask's scale determines how far can the character sprite exit "
               + "the sector's exterior limits.")]
        [SerializeField] public float SpriteMaskRate;

        [Tooltip("The scale of the sector's embedded character sprite, "
               + "relative to the its default size.")]
        [SerializeField] public float SpriteScale;
    }
}