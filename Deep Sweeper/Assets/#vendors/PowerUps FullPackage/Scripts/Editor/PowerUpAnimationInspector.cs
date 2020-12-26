using UnityEngine;
using UnityEditor;

namespace VisCircle
{
    [CustomEditor(typeof(LootAnimation))]
    public class PowerupAnimationInspector : Editor
    {
        public override void OnInspectorGUI() {
            LootAnimation simpleAnimation = target as LootAnimation;

            EditorGUI.BeginChangeCheck();
            bool newAnimateRotation = EditorGUILayout.Toggle("Animated Rotation", simpleAnimation.AnimateRotation);
            if (EditorGUI.EndChangeCheck()) {
                simpleAnimation.AnimateRotation = newAnimateRotation;
            }

            if (simpleAnimation.AnimateRotation) {
                EditorGUI.indentLevel++;
                simpleAnimation.RotationDegrees = EditorGUILayout.Vector3Field("Rotation Speeds", simpleAnimation.RotationDegrees);
                simpleAnimation.AnimationRotationType = (LootAnimation.RotationType)EditorGUILayout.EnumPopup("Rotation Axis", simpleAnimation.AnimationRotationType);
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(10f);

            EditorGUI.BeginChangeCheck();
            bool newAnimateScale = EditorGUILayout.Toggle("Animated Scale", simpleAnimation.AnimateScale);
            if (EditorGUI.EndChangeCheck()) {
                simpleAnimation.AnimateScale = newAnimateScale;
            }

            if (simpleAnimation.AnimateScale) {
                EditorGUI.indentLevel++;
                simpleAnimation.MinScale = EditorGUILayout.FloatField("Min Scale", simpleAnimation.MinScale);
                simpleAnimation.MaxScale = EditorGUILayout.FloatField("Max Scale", simpleAnimation.MaxScale);
                simpleAnimation.ScaleCycleDuration = EditorGUILayout.FloatField("Scale Cycle Duration", simpleAnimation.ScaleCycleDuration);
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(10f);

            EditorGUI.BeginChangeCheck();
            bool newAnimateYOffset = EditorGUILayout.Toggle("Animated Y Offset", simpleAnimation.AnimateOffset);
            if (EditorGUI.EndChangeCheck()) {
                simpleAnimation.AnimateOffset = newAnimateYOffset;
            }

            if (simpleAnimation.AnimateOffset) {
                EditorGUI.indentLevel++;
                simpleAnimation.OffsetAmplitude = EditorGUILayout.FloatField("Amplitude", simpleAnimation.OffsetAmplitude);
                simpleAnimation.OffsetCycleDuration = EditorGUILayout.FloatField("Y Offset Cycle Duration", simpleAnimation.OffsetCycleDuration);
                EditorGUI.indentLevel--;
            }

            if (GUI.changed) {
                EditorUtility.SetDirty(target);
            }
        }
    }

}
