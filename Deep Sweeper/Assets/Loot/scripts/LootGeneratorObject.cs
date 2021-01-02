﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class LootGeneratorObject : MonoBehaviour
{
    protected class LootItemPromise
    {
        #region Class Members
        private GameObject parent;
        private LootGeneratorObject generator;
        private LootItem prefab;
        private Vector3 position;
        #endregion

        /// <param name="generator">The loot generator that creates this promise</param>
        /// <param name="prefab">The prefab of the loot item to instantiate</param>
        /// <param name="pos">The intended position of the loot instance</param>
        public LootItemPromise(LootGeneratorObject generator, LootItem prefab, Vector3 pos) {
            this.parent = LootManager.Instance.gameObject;
            this.generator = generator;
            this.prefab = prefab;
            this.position = pos;
        }

        /// <summary>
        /// Create an instance of the loot.
        /// </summary>
        public void Resolve() {
            GameObject instance = Instantiate(prefab.gameObject);
            instance.transform.SetParent(parent.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.position = position;
            instance.transform.localScale = Vector3.zero;

            LootItem instanceCmp = instance.GetComponent<LootItem>();
            instanceCmp.Generator = generator;
            generator.Item = instanceCmp;
        }
    }

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

    #region Class Members
    protected GameObject lootParent;
    protected GameObject lootItem;
    protected GameObject m_itemObj;
    protected GameObject prevItem;
    protected LootItem m_item;
    protected LootItemPromise itemPromise;
    protected Vector3 originScale;
    protected bool m_enabled;
    protected UnityAction collectAction;
    #endregion

    #region Events
    public event UnityAction CollectedEvent;
    #endregion

    #region Properties
    public LayerMask CollideableLayers { get { return collidableLayers; } }
    public float Chance {
        get { return dropChance * 100; }
        set { dropChance = Mathf.Clamp(value, 0f, 1f); }
    }

    public LootItem Item {
        get { return m_item; }
        set { m_item = value; }
    }

    public bool Enabled {
        get { return m_enabled; }
        protected set {
            if (m_enabled != value && itemPromise != null) {
                if (value) itemPromise.Resolve(); //create loot
                m_enabled = value;
                StopAllCoroutines();
                StartCoroutine(Scale(value));
            }
        }
    }
    #endregion

    protected virtual void Awake() {
        this.m_enabled = false;
        this.lootParent = LootManager.Instance.gameObject;
        this.originScale = Vector3.one * scale;
    }

    protected virtual void Start() {
        if (item != null) SetPrefab(item);
    }

    protected virtual void OnValidate() {
        if (Application.isPlaying && lootParent != null) SetPrefab(item);
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
    }

    /// <summary>
    /// Dispose the item.
    /// </summary>
    /// <param name="collectingLayer">The layer of the object that had collected the item</param>
    /// <param name="dispose">True to dispose the loot item</param>
    public virtual void Collect(int collectingLayer, bool dispose = true) {
        Enabled = false;
        CollectedEvent?.Invoke();
        TakeEffect(collectingLayer);
        if (dispose) Dispose();
    }

    /// <summary>
    /// Dispose the loot item.
    /// </summary>
    public virtual void Dispose() { Destroy(Item.gameObject); }

    /// <summary>
    /// Drop and expose the item.
    /// This method works at a random rate based on 'dropChance'.
    /// </summary>
    public virtual void Drop() {
        if (!Enabled && ChanceUtils.UnstableCondition(dropChance)) Enabled = true;
    }

    /// <summary>
    /// Create a promise object that contains the information
    /// needed to create a loot at runtime.
    /// </summary>
    /// <param name="prefab">The prefab of the loot item</param>
    protected void SetPrefab(LootItem prefab) {
        if (prevItem == prefab.gameObject) return;

        //create an item promise
        Vector3 pos = transform.position + positionOffset;
        itemPromise = new LootItemPromise(this, prefab, pos);
        prevItem = prefab.gameObject;

        //drop the item if it's set to auto drop
        if (!Enabled && autoDrop) Drop();
        else BindDropEvent(Drop);
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