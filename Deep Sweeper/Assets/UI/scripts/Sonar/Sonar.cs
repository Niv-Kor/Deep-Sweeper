using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("A single wave prefab.")]
    [SerializeField] private SonarScanWave scanWavePrefab;

    [Tooltip("The waves' parent object.")]
    [SerializeField] private GameObject wavesParent;

    [Header("Scan Settings")]
    [Tooltip("The time it takes to complete a single scan (in seconds).")]
    [SerializeField] private float scanTime;

    [Tooltip("Delay time in between two scans (in seconds).")]
    [SerializeField] private float scansDelay;

    [Tooltip("The time it takes the signals to fade after their appearance (in seconds).")]
    [SerializeField] private float signalFadeTime;

    [Header("Wave Settings")]
    [Tooltip("The amount of waves in a single scan.")]
    [SerializeField] private int wavesAmount;

    [Tooltip("The time it takes the next wave to appear within a single scan (in seconds).")]
    [SerializeField] private float wavesDelay;
    #endregion

    #region Class Members
    private List<SonarScanWave> waves;
    #endregion

    private void Start() {
        this.waves = new List<SonarScanWave>();

        //instantiate waves
        for (int i = 0; i < wavesAmount; i++) {
            SonarScanWave instance = Instantiate(scanWavePrefab);
            instance.ScanTime = scanTime;
            instance.SignalFadeTime = signalFadeTime;
            instance.transform.SetParent(wavesParent.transform);
            instance.transform.rotation = Quaternion.identity;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
            waves.Add(instance);
        }

        StartCoroutine(Scan());
    }

    private void OnValidate() {
        //clamp signal time <= scanTime
        signalFadeTime = Mathf.Min(signalFadeTime, scanTime);
    }

    /// <summary>
    /// Spawn the waves one by one at their appropriate time.
    /// </summary>
    private IEnumerator Scan() {
        while (true) {
            for (int i = 0; i < waves.Count; i++) {
                waves[i].Scan();
                float extraDelay = (i == waves.Count - 1) ? scanTime : 0;
                yield return new WaitForSeconds(wavesDelay + extraDelay);
            }
            yield return new WaitForSeconds(scansDelay);
        }
    }
}