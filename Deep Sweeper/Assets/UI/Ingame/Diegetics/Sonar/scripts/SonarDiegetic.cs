using DeepSweeper.Flow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.UI.Ingame.Diegetics.Sonar
{
    public class SonarDiegetic : Diegetic
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("A single wave prefab.")]
        [SerializeField] private SonarScanWave scanWavePrefab;

        [Tooltip("The waves' parent object.")]
        [SerializeField] private GameObject wavesParent;

        [Header("Scan Settings")]
        [Tooltip("The time it takes to complete a single scan [s].")]
        [SerializeField] private float scanTime;

        [Tooltip("Delay time in between two scans [s].")]
        [SerializeField] private float scansDelay;

        [Tooltip("The time it takes the signals to fade after their appearance [s].")]
        [SerializeField] private float signalFadeTime;

        [Header("Wave Settings")]
        [Tooltip("The amount of waves in a single scan.")]
        [SerializeField] private int wavesAmount;

        [Tooltip("The time it takes the next wave to appear within a single scan [s].")]
        [SerializeField] private float wavesDelay;
        #endregion

        #region Class Members
        private List<SonarScanWave> waves;
        #endregion

        protected override void Start() {
            base.Start();

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
            //clamp values
            signalFadeTime = Mathf.Min(signalFadeTime, scanTime);
            scansDelay = Mathf.Max(scansDelay, wavesDelay);
        }

        /// <summary>
        /// Spawn the waves one by one at their appropriate time.
        /// </summary>
        private IEnumerator Scan() {
            while (true) {
                for (int i = 0; i < waves.Count; i++) {
                    waves[i].Scan();
                    yield return new WaitForSeconds(wavesDelay);
                }

                yield return new WaitForSeconds(scansDelay);
            }
        }

        /// <inheritdoc/>
        public override void ResetValue(Phase phase) {}

        /// <inheritdoc/>
        public override void OnPhaseStarts(Phase phase) { Activate(true); }

        /// <inheritdoc/>
        public override void OnPhasePauses(Phase phase) { Activate(false, 0); }

        /// <inheritdoc/>
        public override void OnPhaseResumes(Phase phase) { Activate(true, 0); }

        /// <inheritdoc/>
        public override void OnPhaseEnds(Phase phase, bool success) { Activate(false); }
    }
}