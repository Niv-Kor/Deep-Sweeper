using DeepSweeper.Characters;
using DeepSweeper.Player.ShootingSystem;
using System;
using UnityEngine;

namespace DeepSweeper.UI.Ingame.Sight
{
    [Serializable]
    public struct CrosshairAbility : IAbility
    {
        [Tooltip("The type of gun to which the crosshair belongs.")]
        [SerializeField] public GunType Gun;
    }
}