using DeepSweeper.Flow;
using UnityEngine;

[System.Serializable]
public struct LevelConfig
{
    [Tooltip("The region at which the level takes place.")]
    [SerializeField] public Region Region;

    [Tooltip("The index of the level within the area.")]
    [SerializeField] public int Index;
}