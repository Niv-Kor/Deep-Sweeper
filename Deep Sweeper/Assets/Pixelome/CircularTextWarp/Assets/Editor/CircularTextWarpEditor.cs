using UnityEditor;
using UnityEngine;

namespace Pixelome {
    /// <summary>
    /// Used to draw custom handles and support undo.
    /// </summary>
    [CustomEditor(typeof(CircularTextWarp))]
    public class CircularTextWarpEditor : Editor {
        void OnSceneGUI() {
            CircularTextWarp cirularText = (CircularTextWarp)target;
            RectTransform rectTransform = cirularText.GetComponent<RectTransform>();
            float worldSpaceRadius = (rectTransform.localToWorldMatrix * new Vector3(cirularText.Radius, 0, 0)).x;

            // Show radius handle
            Handles.color = Color.cyan;

            EditorGUI.BeginChangeCheck();
            float changedRadius = Handles.ScaleValueHandle(cirularText.Radius, cirularText.transform.position + (Quaternion.Euler(0, 0, 45) * cirularText.transform.right) * worldSpaceRadius, cirularText.transform.rotation * Quaternion.Euler(-45, 90, 0), HandleUtility.GetHandleSize(cirularText.transform.position), Handles.CubeHandleCap, 1);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(cirularText, "Change Circular Text Radius");
                cirularText.Radius = changedRadius;
            }

            // Show rotation offset handle
            Handles.color = Color.magenta;
            
            EditorGUI.BeginChangeCheck();
            float changedOffset = -Handles.Disc(Quaternion.Euler(0, 0, cirularText.RotationOffset), cirularText.transform.position, Vector3.forward, worldSpaceRadius, false, 0).eulerAngles.z;
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(cirularText, "Change Circular Text Offset");
                cirularText.RotationOffset = changedOffset;
            }
        }
    }
}
