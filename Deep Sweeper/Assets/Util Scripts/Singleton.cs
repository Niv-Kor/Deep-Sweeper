﻿using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    public static T Instance {
        get {
            if (instance == null) instance = FindObjectOfType<T>();
            return instance;
        }
    }
}