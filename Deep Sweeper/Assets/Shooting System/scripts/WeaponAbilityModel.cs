using DeepSweeper.Characters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    [Serializable]
    public struct WeaponAbilityModel : IAbilityModel
    {
        [Tooltip("Primary operation weapon.")]
        [SerializeField] public GunType Primary;

        [Tooltip("Secondary operation weapon.")]
        [SerializeField] public GunType Secondary;
    }
}
