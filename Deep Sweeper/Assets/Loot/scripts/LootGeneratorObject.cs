using Constants;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VisCircle;

public abstract class LootGeneratorObject : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The loot object's prefab.")]
    [SerializeField] protected LootItem item;

    [Header("Settings")]
    [Tooltip("The layers that may collide with the loot object.")]
    [SerializeField] protected LayerMask collidableLayers;

    [Tooltip("The offset of the item relative to this object.")]
    [SerializeField] private Vector3 positionOffset;

    [Tooltip("The scale of the loot item (relative to its original size - 1 is 100%).")]
    [SerializeField] private float scale = 1;

    [Header("Chance")]
    [Tooltip("True to drop the item automatically.")]
    [SerializeField] protected bool autoDrop;

    [Tooltip("The chance of the loot being dropped when Drop() is activated.")]
    [SerializeField] [Range(0f, 1f)] private float dropChance = 1;

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
    protected GameObject m_itemObj;
    protected GameObject prevItem;
    protected LootItem m_item;
    protected Vector3 originScale;
    protected bool m_enabled;
    #endregion

    #region Events
    public event UnityAction CollectedEvent;
    #endregion

    #region Properties
    public LayerMask CollideableLayers { get { return collidableLayers; } }
    public LootItem Item {
        get { return m_item; }
        set {
            GameObject itemObj = value.gameObject;
            if (prevItem == itemObj) return;

            //remove existing items from parent
            LootItem[] existingItems = GetComponentsInChildren<LootItem>();
            foreach (LootItem child in existingItems) Destroy(child);

            //create an instance of the item
            m_itemObj = Instantiate(itemObj);
            m_itemObj.transform.SetParent(lootParent.transform);
            m_itemObj.transform.localPosition = Vector3.zero;
            m_itemObj.transform.position += positionOffset;
            m_itemObj.transform.localScale = Vector3.zero;
            m_item = m_itemObj.GetComponent<LootItem>();
            m_item.Generator = this;
            m_item.CollisionEvent += Collect;
            originScale = Vector3.one * scale;
            prevItem = m_itemObj;

            //drop the item if it's set to auto drop
            if (!Enabled && autoDrop) Drop();
            else BindDropEvent(Drop);
        }
    }

    public bool Enabled {
        get { return m_enabled; }
        protected set {
            if (m_enabled != value) {
                m_enabled = value;
                StopAllCoroutines();
                StartCoroutine(Scale(value));
            }
        }
    }
    #endregion

    protected virtual void Awake() {
        this.m_enabled = false;

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
        if (Application.isPlaying && lootParent != null) Item = item;
    }

    /// <summary>
    /// Scale the loot object up (from 0) or down (to 0).
    /// </summary>
    /// <param name="scaleIn">True to scale the loot up of false to scale it down</param>
    protected virtual IEnumerator Scale(bool scaleIn) {
        Vector3 fromSize = scaleIn ? Vector3.zero : Item.transform.localScale;
        Vector3 toSize = scaleIn ? originScale : Vector3.zero;
        float time = scaleIn ? inScaleTime : outScaleTime;
        float timer = 0;

        while (timer <= time) {
            timer += Time.deltaTime;
            Vector3 scale = Vector3.Lerp(fromSize, toSize, timer / time);
            Item.transform.localScale = scale;
            yield return null;
        }

        if (!scaleIn) Destroy(Item.gameObject);
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
    /// Drop and expose the item.
    /// This method works at a random rate based on 'dropChance'.
    /// </summary>
    public virtual void Drop() {
        if (!Enabled && ChanceUtils.UnstableCondition(dropChance)) Enabled = true;
    }

    /// <summary>
    /// This method determines the effect of the loot when collected.
    /// </summary>
    /// <param name="collectingLayer">The layer of the object that had collected the item</param>
    protected abstract void TakeEffect(int collectingLayer);

    /// <summary>
    /// Bind the drop action to an event.
    /// Leave this method empty if the drop trigger is automatic.
    /// <see cref="Drop"/>
    /// </summary>
    /// <param name="dropAction">The function that drops the loot item (at a fixed chance).</param>
    protected abstract void BindDropEvent(UnityAction dropAction);
}