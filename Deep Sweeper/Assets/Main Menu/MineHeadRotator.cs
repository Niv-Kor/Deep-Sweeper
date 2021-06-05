using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.UI.MainMenu
{
    public class MineHeadRotator : Singleton<MineHeadRotator>
    {
        #region Exposed Editor Parameters
        [Tooltip("The speed at which the mine rotates towards the targeted quaternions.")]
        [SerializeField] private float targetedRotSpeed;
        #endregion

        #region Constants
        private static readonly float TARGETED_ROT_TOLERANCE = 10;
        private static readonly Vector3 INITIAL_CONST_ROT_ANGLE = Vector3.one * .01f;
        #endregion

        #region Class Members
        private IDictionary<MenuButton, Quaternion> rotations;
        private Coroutine rotationCoroutine;
        private Coroutine constRotationCoroutine;
        #endregion

        protected override void Awake() {
            base.Awake();
            this.rotations = new Dictionary<MenuButton, Quaternion>();
        }

        private void Start() {
            constRotationCoroutine = StartCoroutine(ConstantRotate(INITIAL_CONST_ROT_ANGLE));
        }

        /// <summary>
        /// Subscribe a menu button and generate a random quaternion to it.
        /// Whenever hovering that button, the mine will rotate towards the generated quaternion.
        /// </summary>
        /// <param name="button">The subscribing menu button</param>
        public void Subscribe(MenuButton button) {
            rotations.Add(button, VectorUtils.GenerateRotation());
            button.HoveredEvent += ChangeRotation;
        }

        /// <summary>
        /// Unubscribe a menu button from the dynamic quaternions structure.
        /// The mine will no longer rotate towards the generated quaternion when hovering that button.
        /// </summary>
        /// <param name="button">The unsubscribing menu button</param>
        public void Unsubscribe(MenuButton button) {
            rotations.Remove(button);
        }

        /// <summary>
        /// Slerp the mine towards a specific rotation until it gets there.
        /// </summary>
        /// <param name="rotation">The direction towards which to rotate the mine</param>
        private IEnumerator Rotate(Quaternion rotation) {
            if (constRotationCoroutine != null) StopCoroutine(constRotationCoroutine);

            Vector3 rotDirection;
            bool progress;

            do {
                //rotate mine
                float step = Time.deltaTime * targetedRotSpeed;
                Quaternion from = transform.rotation;
                transform.rotation = Quaternion.Slerp(from, rotation, step);
                rotDirection = Vector3.Normalize(rotation.eulerAngles - transform.rotation.eulerAngles);

                //check if rotation is effectively over
                Vector3 fromEuler = from.eulerAngles;
                Vector3 toEuler = rotation.eulerAngles;
                progress = !VectorUtils.EffectivelyReached(fromEuler, toEuler, TARGETED_ROT_TOLERANCE);

                yield return null;
            }
            while (progress);

            constRotationCoroutine = StartCoroutine(ConstantRotate(rotDirection * .01f));
        }

        /// <summary>
        /// Constantly rotate towards a specific direction.
        /// </summary>
        /// <param name="direction">The direction towards which to rotate the mine</param>
        private IEnumerator ConstantRotate(Vector3 direction) {
            while (true) {
                transform.Rotate(direction);
                yield return null;
            }
        }

        /// <summary>
        /// Rotate the mine towards a pre-generated menu button quaternion.
        /// This function does nothing if the given button is not already subscribed.
        /// </summary>
        /// <param name="button">The button for which the quaternion was generated</param>
        private void ChangeRotation(MenuButton button) {
            if (!rotations.TryGetValue(button, out Quaternion rotation)) return;

            if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
            rotationCoroutine = StartCoroutine(Rotate(rotation));
        }
    }
}