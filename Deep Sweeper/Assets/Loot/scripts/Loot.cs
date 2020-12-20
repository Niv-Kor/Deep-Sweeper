using Constants;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VisCircle;

public abstract class Loot : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The loot object's prefab.")]
    [SerializeField] protected GameObject item;

    [Header("Settings")]
    [Tooltip("The layers that may collide with the loot object.")]
    [SerializeField] protected LayerMask collidableLayer;

    [Tooltip("True to pop the item automatically.")]
    [SerializeField] protected bool autoPop;

    [Header("Timing")]
    [Tooltip("The time it takes the loot to scale up when popped (in seconds).")]
    [SerializeField] protected float inScaleTime = 0;

    [Tooltip("The time it takes the loot to scale up when disposed (in seconds).")]
    [SerializeField] protected float outScaleTime = 0;
    #endregion

    #region Constants
    protected static readonly string LOOT_PARENT_NAME = "Loots";
    #endregion

    #region Class Members
    protected GameObject lootParent;
    protected GameObject lootItem;
    protected GameObject m_item;
    protected Vector3 originLootSize;
    protected bool m_enabled;
    private GameObject prevItem;
    #endregion

    #region Events
    public event UnityAction CollectedEvent;
    #endregion

    #region Properties
    public GameObject Item {
        get { return m_item; }
        set {
            if (prevItem == value) return;

            LootAnimation lootCmp;
            try { lootCmp = value.GetComponentInChildren<LootAnimation>(); }
            catch { return; }

            Renderer render = lootCmp.GetComponent<Renderer>();
            originLootSize = render.bounds.size;

            //remove existing items from parent
            foreach (Transform child in transform) Destroy(child);

            //create an instance of the item
            m_item = Instantiate(value);
            m_item.transform.SetParent(lootParent.transform);
            m_item.transform.localPosition = Vector3.zero;
            m_item.transform.localScale = Vector3.one;

            prevItem = value;

            //pop the item if it's set to auto pop
            if (!Enabled && autoPop) Pop();
        }
    }

    public bool Enabled {
        get { return m_enabled; }
        protected set {
            m_enabled = value;
            StopAllCoroutines();
            StartCoroutine(Scale(value));
        }
    }
    #endregion

    protected virtual void Awake() {
        //create the parent of the loot objects
        this.lootParent = new GameObject(LOOT_PARENT_NAME);
        lootParent.transform.SetParent(transform);
        lootParent.transform.localPosition = Vector3.zero;
        lootParent.transform.localScale = Vector3.one;
    }

    protected virtual void Start() {
        if (item != null) Item = item;
    }

    protected virtual void OnValidate() {
        if (Application.isPlaying) Item = item;
    }

    protected virtual void OnCollisionEnter(Collision collision) {
        int collisionLayer = collision.gameObject.layer;
        if (Layers.ContainedInMask(collisionLayer, collidableLayer)) Collect(collisionLayer);
    }

    /// <summary>
    /// Scale the loot object up (from 0) or down (to 0).
    /// </summary>
    /// <param name="scaleIn">True to scale the loot up of false to scale it down</param>
    protected virtual IEnumerator Scale(bool scaleIn) {
        Vector3 fromSize = scaleIn ? Vector3.zero : originLootSize;
        Vector3 toSize = scaleIn ? item.transform.localScale : Vector3.zero;
        float time = scaleIn ? inScaleTime : outScaleTime;
        float timer = 0;

        while (timer <= time) {
            timer += Time.deltaTime;
            Vector3 scale = Vector3.Lerp(fromSize, toSize, timer / time);
            item.transform.localScale = scale;
            yield return null;
        }
    }

    /// <summary>
    /// Dispose the item.
    /// </summary>
    /// <param name="collectingLayer">The layer of the object that had collected the item</param>
    protected virtual void Collect(int collectingLayer) {
        Enabled = false;
        CollectedEvent?.Invoke();
        TakeEffect(collectingLayer);
    }

    /// <summary>
    /// Expose the item.
    /// </summary>
    public virtual void Pop() {
        Enabled = true;
    }

    /// <summary>
    /// This method determines the effect of the loot when collected.
    /// </summary>
    /// <param name="collectingLayer">The layer of the object that had collected the item</param>
    protected abstract void TakeEffect(int collectingLayer);
}