using Constants;
using UnityEngine;

namespace DeepSweeper.UI.Ingame.Diegetics.Sonar
{
    public class MinimapActor : MonoBehaviour
    {
        protected enum MapLayer
        {
            Player,
            Mine
        }

        #region Exposed editor parameters
        [Header("Prefabs")]
        [Tooltip("The sprite that represents this object in the minimap.")]
        [SerializeField] protected Sprite sprite;

        [Tooltip("A prefab of an object to use as the minimap icon\n"
               + "(optional - you must either choose a sprite or a prefab).")]
        [SerializeField] protected GameObject iconPrefab;

        [Header("Settings")]
        [Tooltip("The layer that this icon belongs to.")]
        [SerializeField] protected MapLayer layer;

        [Tooltip("True to immediately display the icon in the Minimap layer.")]
        [SerializeField] protected bool immediateDisplay = true;

        [Header("View")]
        [Tooltip("The size of the icon as viewed over the minimap.")]
        [SerializeField] protected float size = 1;

        [Tooltip("The color that's applied to the icon.")]
        [SerializeField] protected Color defaultColor;
        #endregion

        #region Constants
        protected static readonly string OBJECT_NAME = "Minimap Icon";
        #endregion

        #region Class members
        protected GameObject iconObj;
        protected SpriteRenderer spriteRenderer;
        #endregion

        #region properties
        public Vector2 Size {
            get { return (spriteRenderer != null) ? spriteRenderer.size : Vector2.zero; }
            set { if (spriteRenderer != null) spriteRenderer.size = value; }
        }

        public Sprite Sprite {
            get { return spriteRenderer?.sprite; }
            set {
                if (spriteRenderer != null)
                    spriteRenderer.sprite = value;
            }
        }
        #endregion

        protected virtual void Awake() {
            this.iconObj = InstantiateIconObj(sprite != null);
            ApplyMinimapLayer(immediateDisplay);
        }

        protected virtual void OnValidate() {
            if (spriteRenderer != null && sprite != null) {
                Sprite = sprite;
                Size = Vector2.one * size;
            }
        }

        /// <summary>
        /// Create the minimap child object.
        /// </summary>
        /// <param name="useSprite">
        /// True to create a seperate object with its own sprite renderer,
        /// or false to clone this object and display it in the minimap camera.
        /// </param>
        /// <returns>The instantiated object.</returns>
        protected virtual GameObject InstantiateIconObj(bool useSprite) {
            if ((useSprite && sprite == null) || (!useSprite && iconPrefab == null)) return null;

            GameObject iconObj = useSprite ? new GameObject(OBJECT_NAME) : Instantiate(iconPrefab);
            MinimapIcon iconCmp = iconObj.AddComponent<MinimapIcon>();
            iconCmp.YawAngleFunc = GetYawAngle;

            if (useSprite) {
                iconObj.transform.SetParent(transform);
                iconObj.transform.localPosition = Vector3.zero;

                this.spriteRenderer = iconObj.AddComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = layer.ToString();
                spriteRenderer.sortingOrder = GetSortingOrder(layer);
                spriteRenderer.color = defaultColor;
                spriteRenderer.drawMode = SpriteDrawMode.Sliced;
                Sprite = sprite;
                Size = Vector2.one * size;
            }
            else {
                iconObj.transform.SetParent(transform.parent);
                iconObj.transform.localPosition = Vector3.zero;
                Destroy(iconObj.GetComponent<MinimapActor>());
            }
            return iconObj;
        }

        /// <param name="layer">The icon's layer</param>
        /// <returns>The laryer's sort order.</returns>
        protected virtual int GetSortingOrder(MapLayer layer) {
            switch (layer) {
                case MapLayer.Player: return 2;
                case MapLayer.Mine: return 1;
                default: return 0;
            }
        }

        /// <returns>The current yaw angle of the icon.</returns>
        protected virtual float GetYawAngle() {
            return transform.rotation.eulerAngles.y;
        }

        /// <summary>
        /// Display or hide the icon in the minmap.
        /// </summary>
        /// <param name="flag">True to display the icon in the minimap or false to hide</param>
        protected virtual void ApplyMinimapLayer(bool flag) {
            LayerMask mask = flag ? Layers.MINIMAP : Layers.TRANSPARENT_FX;
            iconObj.layer = Layers.GetLayerValue(mask);
        }
    }
}