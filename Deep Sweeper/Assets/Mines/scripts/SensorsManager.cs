using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Level.Mine
{
    public class SensorsManager : MonoBehaviour
    {
        #region Class Members
        private int maxAmount;
        #endregion

        #region Events
        public event UnityAction AllSensorsBrokenEvent;
        #endregion

        #region Properties
        public List<MineSensor> Sensors { get; private set; }
        public float RemainSensorsPercent => CurrentAmount / maxAmount;
        public int CurrentAmount => Sensors.Count;
        #endregion

        private void Awake() {
            this.Sensors = new List<MineSensor>(GetComponentsInChildren<MineSensor>());
            this.maxAmount = Sensors.Count;
        }

        /// <summary>
        /// Break a single random mine sensor.
        /// </summary>
        /// <param name="triggerExplosion">True to trigger an explosion event once the last sensor break</param>
        private void BreakRandomSensor() {
            int index = Random.Range(0, CurrentAmount);
            MineSensor sensor = Sensors[index];
            sensor.Break();
            Sensors.Remove(sensor);
            if (CurrentAmount == 0)
                AllSensorsBrokenEvent?.Invoke();
        }

        /// <summary>
        /// Break a constant amount of mine sensors.
        /// If the specified amount is larger than the remaining available
        /// sensors, all sensors will be broken.
        /// </summary>
        /// <param name="amount">The amount of sensors to break</param>
        /// <param name="triggerExplosion">True to trigger an explosion event once the last sensor break</param>
        public void BreakSensors(int amount) {
            int availableAmount = CurrentAmount;

            for (int i = 0; i < Mathf.Min(amount, availableAmount); i++)
                BreakRandomSensor();
        }

        /// <summary>
        /// Break a percentage of the full amount of sensors.
        /// </summary>
        /// <param name="percent">A percentage of the maximum sensors this mine had in the beginning</param>
        /// <param name="triggerExplosion">True to trigger an explosion event once the last sensor break</param>
        /// <returns>The exact amount of broken sensors in practice.</returns>
        public int BreakSensors(float percent) {
            int amount = (int) (percent * maxAmount);
            amount = Mathf.Max(amount, 1);
            BreakSensors(amount);
            return Mathf.Min(amount, maxAmount);
        }
    }
}