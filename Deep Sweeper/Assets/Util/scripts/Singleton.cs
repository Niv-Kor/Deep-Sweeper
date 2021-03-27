using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Class Members
    protected static T instance;
    #endregion

    #region Properties
    public static T Instance {
        get {
            if (instance == null) {
                try { instance = FindObjectOfType<T>(); }
                catch (NullReferenceException) { return null; }
            }
            return instance;
        }
    }
    #endregion
}