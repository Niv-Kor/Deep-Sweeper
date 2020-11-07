using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;

public class GateElectrodes : MonoBehaviour
{
    [Tooltip("The color of the led when the gate is open.")]
    [SerializeField] private Color openLed;

    [Tooltip("The color of the led when the gate is closed.")]
    [SerializeField] private Color closedLed;

    [Tooltip("The time it takes to change the leds color.")]
    [SerializeField] private float colorChangeTime;

    private static readonly Color TRANSPARENT = new Color(0xff, 0xff, 0xff, 0);

    private Light[] leds;

    public Color LedsColor {
        get {
            if (leds.Length == 0) return TRANSPARENT;
            else return leds[0].color;
        }
        set {
            StopAllCoroutines();
            StartCoroutine(SetLedsColor(value));
        }
    }
    
    /// <summary>
    /// Initialize the component.
    /// </summary>
    /// <param name="gateCmp">Parent gate component</param>
    public void Initiate(Gate gateCmp) {
        this.leds = GetComponentsInChildren<Light>();
        gateCmp.GateOpenEvent += delegate {
            LedsColor = openLed;
        };
        LedsColor = closedLed;
    }

    /// <param name="color">The new color of the leds</param>
    private IEnumerator SetLedsColor(Color color) {
        if (leds.Length == 0) yield break;

        Color startColor = LedsColor;
        float timer = 0;

        while (timer <= colorChangeTime) {
            timer += Time.deltaTime;
            Color nextColor = Color.Lerp(startColor, color, timer / colorChangeTime);
            foreach (Light led in leds) led.color = nextColor;
            yield return null;
        }
    }
}