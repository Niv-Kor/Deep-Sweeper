using DeepSweeper.Characters;
using DeepSweeper.UI.Ingame.Spatials.Commander;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceCircle : MonoBehaviour
{
    [Serializable]
    private struct RaceSprite
    {
        [Tooltip("The accosiated tribal race.")]
        [SerializeField] public TribalRace Race;

        [Tooltip("A plain sprite of the race's sybmol.")]
        [SerializeField] public Texture Plain;

        [Tooltip("A glowing sprite of the race's symbol.")]
        [SerializeField] public Texture Glow;
    }

    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("A list of race sprites configurations.")]
    [SerializeField] private List<RaceSprite> spritesConfig;

    [Tooltip("The plain race symbol's sprite component.")]
    [SerializeField] private RawImage PlainCmp;

    [Tooltip("The glowing race symbol's sprite component.")]
    [SerializeField] private RawImage GlowCmp;

    [Header("Glow Settings")]
    [Tooltip("The speed at which the race symbol glows.")]
    [SerializeField] private float glowSpeed = 1;

    [Tooltip("The minimum glow value of the race symbol.")]
    [SerializeField] [Range(0f, 1f)] private float minGlow = 0;

    [Tooltip("The maximum glow value of the race symbol.")]
    [SerializeField] [Range(0f, 1f)] private float maxGlow = 1;
    #endregion

    #region Class Members
    private Coroutine glowCoroutine;
    private SectorialDivisor sectorialDivisor;
    private TribalRace m_race;
    #endregion

    #region Properties
    public TribalRace Race {
        get => m_race;
        set {
            if (m_race != value) {
                RaceSprite sprite = spritesConfig.Find(x => x.Race == value);

                if (sprite.Race != TribalRace.None) {
                    PlainCmp.texture = sprite.Plain;
                    GlowCmp.texture = sprite.Glow;
                    m_race = value;
                }
            }
        }
    }
    #endregion

    private void Awake() {
        CommanderSpatial spatial = GetComponentInParent<CommanderSpatial>();
        this.sectorialDivisor = spatial.GetComponentInChildren<SectorialDivisor>();
        sectorialDivisor.SectorSelectedEvent += OnSectorSelected;
        spatial.ActivatedEvent += OnSpatialActivated;
    }

    private void OnValidate() {
        maxGlow = Mathf.Max(minGlow, maxGlow);

        //assert all races are well configures
        foreach (TribalRace race in Enum.GetValues(typeof(TribalRace))) {
            if (spritesConfig.FindIndex(x => x.Race == Race) == -1) {
                spritesConfig.Add(new RaceSprite {
                    Race = race,
                    Plain = null,
                    Glow = null
                });
            }
        }
    }

    /// <summary>
    /// Activate when a sector is temporarily selected.
    /// This method changes the race symbol to the one that
    /// corresponds to the selected character's race.
    /// </summary>
    /// <param name="sector">The selected sector</param>
    private void OnSectorSelected(SectorManager sector) {
        Race = sector.Character.Race();
    }

    /// <summary>
    /// Activate when the spatial activated or deactivates.
    /// This method starts or stops the glow animation accordingly.
    /// </summary>
    /// <param name="flag">True if the spatial activates or false if it deactivates</param>
    private void OnSpatialActivated(bool flag) {
        if (flag) {
            if (glowCoroutine != null) StopCoroutine(glowCoroutine);
            glowCoroutine = StartCoroutine(LetGlow());
        }
        else {
            if (glowCoroutine != null) StopCoroutine(glowCoroutine);
            Color tempColor = GlowCmp.color;
            tempColor.a = minGlow;
            GlowCmp.color = tempColor;
        }
    }

    /// <summary>
    /// Start the race's symbol glow animation.
    /// </summary>
    private IEnumerator LetGlow() {
        float timer = 0;
        Color color = GlowCmp.color;

        while (true) {
            timer += Time.deltaTime * glowSpeed;
            float sineWave = Mathf.Sin(timer - 1) + 1;
            float percent = RangeMath.NumberOfRange(sineWave, new Vector2(0, 2));
            float stepAlpha = RangeMath.PercentOfRange(percent, new Vector2(minGlow, maxGlow));
            color.a = stepAlpha;
            GlowCmp.color = color;

            yield return null;
        }
    }
}