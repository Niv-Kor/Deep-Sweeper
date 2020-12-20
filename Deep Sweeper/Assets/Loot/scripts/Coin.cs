using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Loot
{
    protected override void TakeEffect(int collectingLayer) {
        print("collected coin");
    }
}