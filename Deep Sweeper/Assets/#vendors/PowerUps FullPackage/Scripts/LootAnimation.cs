using UnityEngine;

namespace VisCircle
{
    public class LootAnimation : MonoBehaviour
    {
        public enum RotationType {
            SelfAxis,
            WorldAxis
        }

        #region Exposed Editor Parameters
        [Header("Rotation")]
        [Tooltip("True to rotate the loot item.")]
        [SerializeField] private bool animateRotation = true;

        [Tooltip("The degrees of rotation per second.")]
        [SerializeField] private Vector3 rotationDegrees;

        [Tooltip("The type of rotation (whether self or world axis rotation).")]
        [SerializeField] private RotationType rotationType = RotationType.SelfAxis;

        [Header("Scale")]
        [Tooltip("True to scale the loot item up and down.")]
        [SerializeField] private bool animateScale = true;

        [Tooltip("The minimum and maximum scale change values of the item.")]
        [SerializeField] private Vector2 scaleRange = new Vector2(0.5f, 1.5f);

        [Tooltip("The time it takes to scale down and up again.")]
        [SerializeField] private float scaleCycleDuration = 5;

        [Header("Offset")]
        [Tooltip("True to apply a Y axis offset animation to the item.")]
        [SerializeField] private bool animateYOffset = true;

        [Tooltip("The entire amplitude of the Y axis offset.")]
        [SerializeField] private float yOffsetAmplitude = 1;

        [Tooltip("The cyclic time it takes to move the item across the Y axis.")]
        [SerializeField] private float yOffsetCycleDuration = 5;
        #endregion

        #region Class Members
        private Vector3 originPos;
        private Quaternion originRot;
        private Vector3 originScale;
        private Space spaceAxis;
        private float offsetTime;
        #endregion

        #region Properties
        public bool AnimateRotation {
            get { return animateRotation; }
            set {
                animateRotation = value;

                //restore scale
                if (!animateRotation && Application.isPlaying)
                    transform.localRotation = originRot;
            }
        }

        public Vector3 RotationDegrees {
            get { return rotationDegrees; }
            set { rotationDegrees = value; }
        }

        public RotationType AnimationRotationType {
            get { return rotationType; }
            set { rotationType = value; }
        }

        public float MinScale {
            get { return scaleRange.x; }
            set { scaleRange.x = value; }
        }

        public float MaxScale {
            get { return scaleRange.y; }
            set { scaleRange.y = value; }
        }

        public float ScaleCycleDuration {
            get { return scaleCycleDuration; }
            set { scaleCycleDuration = value; }
        }

        public bool AnimateScale {
            get { return animateScale; }
            set {
                animateScale = value;

                //restore scale
                if (!animateScale && Application.isPlaying)
                    transform.localScale = originScale;
            }
        }

        public bool AnimateOffset {
            get { return animateYOffset; }
            set {
                animateYOffset = value;

                //restore scale
                if (!animateYOffset && Application.isPlaying)
                    transform.localRotation = originRot;
            }
        }

        public float OffsetAmplitude {
            get { return yOffsetAmplitude; }
            set { yOffsetAmplitude = value; }
        }

        public float OffsetCycleDuration {
            get { return yOffsetCycleDuration; }
            set { yOffsetCycleDuration = value; }
        }
        #endregion

        void Start() {
            this.originPos = transform.localPosition;
            this.originRot = transform.localRotation;
            this.originScale = transform.localScale;

            //initial rotation
            switch (rotationType) {
                case RotationType.WorldAxis: spaceAxis = Space.World; break;
                case RotationType.SelfAxis: spaceAxis = Space.Self; break;
                default: spaceAxis = Space.Self; break;
            }

            transform.Rotate(rotationDegrees * Random.value, spaceAxis);

            //initial offset
            this.offsetTime = Random.value * yOffsetAmplitude;
        }

        void Update() {
            //rotation
            if (animateRotation) transform.Rotate(rotationDegrees * Time.deltaTime, spaceAxis);

            //scale
            if (animateScale) {
                float scale = 1;

                if (scaleCycleDuration != 0) {
                    float scaleT = Mathf.InverseLerp(-1, 1, Mathf.Sin(Time.time / scaleCycleDuration * Mathf.PI * 2));
                    scale = Mathf.Lerp(MinScale, MaxScale, scaleT);
                }

                transform.localScale = scale * originScale;
            }

            //Y axis offset
            if (animateYOffset) {
                float yOff = 0;

                if (yOffsetCycleDuration != 0) {
                    offsetTime += Time.deltaTime;
                    float sineWave = Mathf.Sin(offsetTime / yOffsetCycleDuration * Mathf.PI * 2);
                    yOff = sineWave * yOffsetAmplitude;
                }

                transform.localPosition = originPos + Vector3.up * yOff;
            }
        }
    }
}