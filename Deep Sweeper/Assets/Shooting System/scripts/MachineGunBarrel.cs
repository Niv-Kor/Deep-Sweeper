using System.Collections;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public class MachineGunBarrel : Barrel
    {
        #region Constants
        private static readonly float ROTATION_FEEDBACK_TIME = .2f;
        private static readonly string ROTATION_PARAM = "rotate";
        #endregion

        #region Class Members
        private Coroutine rotationCoroutine;
        #endregion

        /// <inheritdoc/>
        public override void Blast() {
            if (useAnimator) puppeteer.Manipulate(ROTATION_PARAM, true);
            base.Blast();

            if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
            rotationCoroutine = StartCoroutine(StopRotation());
        }

        /// <summary>
        /// Stop the rotation of the firearm after a fixed delay.
        /// </summary>
        private IEnumerator StopRotation() {
            yield return new WaitForSeconds(ROTATION_FEEDBACK_TIME);
            puppeteer.Manipulate(ROTATION_PARAM, false);
        }
    }
}