using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Level.Mine
{
    public class SensorsManager : MonoBehaviour
    {
        #region Class Members
        private int maxAmount;
        private int currentAmount;
        #endregion

        #region Events
        public event UnityAction AllSensorsBrokenEvent;
        #endregion

        #region Properties
        public List<MineSensor> Sensors { get; private set; }
        public float RemainSensorsPercent => currentAmount / maxAmount;
        #endregion

        private void Awake() {
            this.Sensors = new List<MineSensor>(GetComponentsInChildren<MineSensor>());
            this.maxAmount = Sensors.Count;
            this.currentAmount = maxAmount;
        }

        /// <summary>
        /// Break a single random mine sensor.
        /// </summary>
        /// <param name="triggerExplosion">True to trigger an explosion event once the last sensor break</param>
        private void BreakRandomSensor(bool triggerExplosion = true) {
            int index = Random.Range(0, currentAmount - 1);
            Sensors[index].Break();
            if (--currentAmount == 0 && triggerExplosion) AllSensorsBrokenEvent?.Invoke();
        }

        /// <summary>
        /// Break a constant amount of mine sensors.
        /// If the specified amount is larger than the remaining available
        /// sensors, all sensors will be broken.
        /// </summary>
        /// <param name="amount">The amount of sensors to break</param>
        /// <param name="triggerExplosion">True to trigger an explosion event once the last sensor break</param>
        public void BreakSensors(int amount, bool triggerExplosion = true) {
            for (int i = 0; i < Mathf.Min(amount, currentAmount); i++)
                BreakRandomSensor(triggerExplosion);
        }

        /// <summary>
        /// Break a percentage of the full amount of sensors.
        /// </summary>
        /// <param name="percent">A percentage of the maximum sensors this mine had in the beginning</param>
        /// <param name="triggerExplosion">True to trigger an explosion event once the last sensor break</param>
        /// <returns>The exact amount of broken sensors in practice.</returns>
        public int BreakSensors(float percent, bool triggerExplosion = true) {
            int amount = (int) (percent * maxAmount);
            BreakSensors(amount, triggerExplosion);
            return Mathf.Min(amount, maxAmount);
        }
    }
}