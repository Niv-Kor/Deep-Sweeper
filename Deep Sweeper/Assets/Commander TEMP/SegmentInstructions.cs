using System;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    [Serializable]
    public struct SegmentInstructions
    {
        [SerializeField] public RadialToolkit.Segment Segment;

        [SerializeField] public Image.Origin360 FillOrigin;

        [SerializeField] public bool Clockwise;

        [SerializeField] public float Roll;
    }
}