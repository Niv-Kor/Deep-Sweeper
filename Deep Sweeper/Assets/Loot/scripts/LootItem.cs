using Constants;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VisCircle;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(LootAnimation))]
[RequireComponent(typeof(Jukebox))]
public class LootItem : MonoBehaviour
{
    #region Class Members
    private Jukebox jukebox;
    private Tune tune;
    private LootGeneratorObject lootGenerator;
    private LayerMask collideableLayers;
    private bool generatorSet;
    private bool collided;
    #endregion

    #region Events
    public event UnityAction<int> CollisionEvent;
    #endregion

    #region Properties
    public LootGeneratorObject Generator {
        set {
            if (!generatorSet) {
                lootGenerator = value;
                collideableLayers = lootGenerator.CollideableLayers;

                //bind events
                CollisionEvent += delegate(int layer) {
                    value.Collect(layer, tune == null);
                };

                if (tune != null) {
                    tune.StopEvent += lootGenerator.Dispose;
                    lootGenerator.CollectedEvent += PlaySound;
                }

                generatorSet = true;
            }
        }
    }
    #endregion

    private void Awake() {
        this.jukebox = GetComponent<Jukebox>();
        List<Tune> collectionTracks = jukebox.Tunes.FindAll(x => x.Genre == Genre.SFX);
        int trackIndex = Random.Range(0, collectionTracks.Count);
        this.tune = (collectionTracks.Count > 0) ? collectionTracks[trackIndex] : null;
        this.generatorSet = false;
        this.collided = false;
    }

    private void OnTriggerEnter(Collider col) {
        if (collided || !col.isTrigger) return;
        int collisionLayer = col.gameObject.layer;

        if (Layers.ContainedInMask(collisionLayer, collideableLayers)) {
            CollisionEvent?.Invoke(collisionLayer);
            collided = true;
        }
    }
    
    /// <summary>
    /// Play the collection track of this loot item.
    /// </summary>
    private void PlaySound() {
        jukebox.Play(tune);
    }
}