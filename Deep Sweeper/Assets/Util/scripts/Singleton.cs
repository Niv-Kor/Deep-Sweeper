using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Class Members
    private static T m_instance;
    #endregion

    #region Properties
    public static T Instance {
        get {
            if (m_instance == null) {
                try { m_instance = FindObjectOfType<T>(); }
                catch (NullReferenceException) { return null; }
            }
            return m_instance;
        }
    }
    #endregion

    protected virtual void Awake() {
        if (m_instance != null && m_instance != this) Destroy(this);
    }
}