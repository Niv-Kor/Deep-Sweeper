using Constants;
using UnityEngine;
using UnityEngine.Events;
using VisCircle;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(LootAnimation))]
public class LootItem : MonoBehaviour
{
    #region Class Members
    private LootGeneratorObject lootGenerator;
    private LayerMask collideableLayers;
    #endregion

    #region Events
    public event UnityAction<int> CollisionEvent;
    #endregion

    #region Properties
    public LootGeneratorObject Generator {
        set {
            lootGenerator = value;
            collideableLayers = lootGenerator.CollideableLayers;
        }
    }
    #endregion

    protected virtual void OnTriggerEnter(Collider col) {
        int collisionLayer = col.gameObject.layer;

        if (Layers.ContainedInMask(collisionLayer, collideableLayers))
            CollisionEvent?.Invoke(collisionLayer);
    }
}