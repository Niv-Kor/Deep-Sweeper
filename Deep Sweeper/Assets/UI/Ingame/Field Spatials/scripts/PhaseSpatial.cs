using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class PhaseSpatial<T> : Singleton<T> where T : MonoBehaviour
{
    #region Class Members
    private List<GameObject> children;
    private bool m_enabled;
    #endregion

    #region Properties
    public bool Enabled {
        get { return m_enabled; }
        protected set {
            m_enabled = value;

            foreach (GameObject child in children)
                child.SetActive(value);
        }
    }
    #endregion

    protected virtual void Start() {
        this.children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        this.Enabled = false;
    }
}