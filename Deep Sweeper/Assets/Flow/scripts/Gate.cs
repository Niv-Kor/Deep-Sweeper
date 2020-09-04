using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public Phase Phase { get; set; }
    public bool IsOpen { get; private set; }
}