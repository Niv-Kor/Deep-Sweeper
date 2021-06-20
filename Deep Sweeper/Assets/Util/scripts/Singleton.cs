using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Class Members
    private static T m_instance = null;
    #endregion

    #region Properties
    public static T Instance {
        get {
            if (m_instance == null) {
                m_instance = FindObjectOfType<T>();
                return m_instance;
            }

            return m_instance;
        }
    }
    #endregion

    protected virtual void Awake() {
        if (Instance != this) Destroy(this);
    }
}