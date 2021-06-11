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
        [SerializeField] public TribalRace Race;
        [SerializeField] public Texture Plain;
        [SerializeField] public Texture Glow;
    }

    #region Exposed Editor Parameters
    [SerializeField] private List<RaceSprite> spritesConfig;
    [SerializeField] private SectorialDivisor sectorialDivisor;
    [SerializeField] private float glowSpeed = 1;
    [SerializeField] [Range(0f, 1f)] private float minGlow = 0;
    [SerializeField] [Range(0f, 1f)] private float maxGlow = 1;
    [SerializeField] private RawImage Plain;
    [SerializeField] private RawImage Glow;
    #endregion

    #region Class Members
    private Coroutine glowCoroutine;
    private TribalRace m_race;
    #endregion

    #region Properties
    public TribalRace Race {
        get => m_race;
        set {
            if (m_race != value) {
                RaceSprite sprite = spritesConfig.Find(x => x.Race == value);

                if (sprite.Race != TribalRace.None) {
                    Plain.texture = sprite.Plain;
                    Glow.texture = sprite.Glow;
                    m_race = value;
                }
            }
        }
    }
    #endregion

    private void Awake() {
        CommanderSpatial spatial = GetComponentInParent<CommanderSpatial>();
        sectorialDivisor.SectorSelectedEvent += OnSectorSelected;
        spatial.ActivatedEvent += OnSpatialActivated;
    }

    private void OnValidate() {
        maxGlow = Mathf.Max(minGlow, maxGlow);
    }

    private void OnSectorSelected(SectorManager sector) {
        Race = sector.Character.Race();
    }

    private void OnSpatialActivated(bool activated) {
        if (activated) {
            if (glowCoroutine != null) StopCoroutine(glowCoroutine);
            glowCoroutine = StartCoroutine(LetGlow());
        }
        else {
            if (glowCoroutine != null) StopCoroutine(glowCoroutine);
            Color tempColor = Glow.color;
            tempColor.a = minGlow;
            Glow.color = tempColor;
        }
    }

    private IEnumerator LetGlow() {
        float timer = 0;
        Color color = Glow.color;

        while (true) {
            timer += Time.deltaTime * glowSpeed;
            float sineWave = Mathf.Sin(timer - 1) + 1;
            float percent = RangeMath.NumberOfRange(sineWave, new Vector2(0, 2));
            float stepAlpha = RangeMath.PercentOfRange(percent, new Vector2(minGlow, maxGlow));
            color.a = stepAlpha;
            Glow.color = color;

            yield return null;
        }
    }
}