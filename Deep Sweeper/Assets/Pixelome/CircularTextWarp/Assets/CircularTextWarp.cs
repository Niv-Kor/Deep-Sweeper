using UnityEngine;
using TMPro;

namespace Pixelome {
    /// <summary>
    /// Circular text warp component for TextMesh Pro.
    /// <para>It needs to be attached to an object that has a <see cref="TMP_Text"/> component.</para>
    /// <para>It works by editing character meshes of a TextMesh Pro component.</para>
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CircularTextWarp : MonoBehaviour {

        #region Public properties
        [SerializeField]
        bool facingInside;
        bool cached_facingInside;
        /// <summary>
        /// Text orientation, weather the warped text should face inside or outside.
        /// </summary>
        public bool FacingInside {
            get { return facingInside; }
            set {
                if (value == cached_facingInside) return;
                SetNeedsUpdate();
                cached_facingInside = facingInside = value;
            }
        }

        [SerializeField]
        float rotationOffset;
        float cached_rotationOffset;
        /// <summary>
        /// Offsets the rotation of text.
        /// <para>For performance, it is better to animate the rect transform rotation, 
        /// since changes to this property will trigger text redraw and warp recalculation.</para>
        /// </summary>
        public float RotationOffset {
            get { return rotationOffset; }
            set {
                if (Mathf.Approximately(value, cached_rotationOffset)) return;
                SetNeedsUpdate();
                cached_rotationOffset = rotationOffset = value;
            }
        }
        [SerializeField]
        float radius = 50f;
        float cached_radius;
        /// <summary>
        /// Radius of the circularly warped text. 
        /// <para>Vertical aligment of the TextMesh Pro component in combination with the RectTransform height, affects the final position of characters.</para>
        /// <para><see cref="TextAlignmentOptions.Baseline"/> places the character on the radius line.</para>
        /// </summary>
        public float Radius {
            get { return radius; }
            set {
                if (Mathf.Approximately(value, cached_radius)) return;
                SetNeedsUpdate();
                cached_radius = radius = value;
            }
        }
        #endregion

        #region Private fields
        bool needsUpdate;
        TMP_Text textComponent;
        RectTransform rectTransform;
        #endregion

        #region Unity events
        void OnValidate() {
            // make sure setters are called for editor changes
            FacingInside = facingInside;
            RotationOffset = rotationOffset;
            Radius = radius;
        }

        void Awake() {
            textComponent = GetComponent<TMP_Text>();
            rectTransform = GetComponent<RectTransform>();
        }
        
        void Update() {
            if (needsUpdate) {
                needsUpdate = false;
                textComponent.ForceMeshUpdate();
            }
        }
        
        void OnEnable() {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
            TMPro_EventManager.FONT_PROPERTY_EVENT.Add(OnFontChanged);
            SetNeedsUpdate();
        }
        
        void OnDisable() {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
            TMPro_EventManager.FONT_PROPERTY_EVENT.Remove(OnFontChanged);
            textComponent.ForceMeshUpdate();
        }
        #endregion

        #region TextMeshPro events
        void OnTextChanged(Object obj) {
            if (obj == textComponent) {
                UpdateMargin();
                CurveText();
            }
        }
        
        void OnFontChanged(bool b, Object font) {
            UpdateMargin();
            CurveText();
        }
        #endregion

        #region Logic
        /// <summary>
        /// Updates the TextMesh Pro components horizontal margins to fit the circle circumference.
        /// <para>Margins are edited instead of the RectTransform width for ease of layout.</para>
        /// <para>If you need the horizontal margins, this method can be changed to set the RectTransform width to "fullArcLength".</para>
        /// </summary>
        void UpdateMargin() {
            float fullArcLength = 2 * Mathf.PI * radius;
            float sizeDiff = (rectTransform.rect.size.x - fullArcLength) / 2f;
            textComponent.margin = new Vector4(sizeDiff, textComponent.margin.y, sizeDiff, textComponent.margin.w);
        }

        /// <summary>
        /// Recalculates the warp on the next update.
        /// <para>Should be called externally only in rare cases where a change doesn't trigger the recaulculation.</para>
        /// </summary>
        public void SetNeedsUpdate() {
            needsUpdate = true;
        }

        /// <summary>
        /// Warps the text. 
        /// <para>Shouldn't be called directly. Use <see cref="SetNeedsUpdate"/> instead if a change didn't trigger a recalculation.</para>
        /// </summary>
        void CurveText() {
            TMP_TextInfo textInfo = textComponent.textInfo;
            TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
            int characterCount = textInfo.characterCount;
            
            if (characterCount == 0) return;
            
            float fullArcLength = 2 * Mathf.PI * radius;
            float textArcLength = textComponent.textBounds.size.x;
            float textAngle = (360 * textArcLength) / fullArcLength;
            
            for (int i = 0; i < characterCount; i++) {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                
                // Skip characters that are not visible and thus have no geometry to manipulate.
                if (!charInfo.isVisible) continue;
                
                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                
                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                
                // Get the cached vertices of the mesh used by this text element (character or sprite).
                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
                
                Vector3 baslineOffset = new Vector3(0, sourceVertices[vertexIndex + 0].y);
                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - baslineOffset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - baslineOffset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - baslineOffset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - baslineOffset;
                
                float rotationAngle = RotationAngle(charInfo, textArcLength, textAngle);
                if (float.IsNaN(rotationAngle) || float.IsInfinity(rotationAngle)) continue;
                float characterRotationAngle = rotationAngle + (facingInside ? 90 : -90);
                float characterDistance = radius + (facingInside ? -baslineOffset.y : baslineOffset.y);
                
                Matrix4x4 originMatrix = Matrix4x4.Translate(OriginOffset(destinationVertices, vertexIndex));
                Matrix4x4 rotateMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, characterRotationAngle));
                Matrix4x4 transformMatrix = Matrix4x4.Translate(CharacterTranslation(rotationAngle, characterDistance));
                Matrix4x4 m = transformMatrix * rotateMatrix * originMatrix;
                
                destinationVertices[vertexIndex + 0] = m.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] = m.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] = m.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] = m.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);
            }
            
            // Push changes into meshes
            for (int i = 0; i < textInfo.meshInfo.Length; i++) {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }

        /// <summary>
        /// Convenience method for getting the offset needed for a character to be returned to the components origin point.
        /// </summary>
        Vector3 OriginOffset(Vector3[] destinationVertices, int vertexIndex) {
            return -destinationVertices[vertexIndex + 0] + (destinationVertices[vertexIndex + 0] - destinationVertices[vertexIndex + 3]) * 0.5f;
        }

        /// <summary>
        /// Convenience method for calculating the angle at wich to offset and rotate a character.
        /// </summary>
        float RotationAngle(TMP_CharacterInfo charInfo, float textArcLength, float textAngle) {
            float charWidth = Mathf.Abs(Mathf.Max(charInfo.origin, charInfo.xAdvance) - Mathf.Min(charInfo.origin, charInfo.xAdvance));
            float angle = (((charInfo.origin + (charWidth / 2.0f)) / textArcLength) * textAngle);
            return facingInside ? (angle + 90) - rotationOffset : (450 - (rotationOffset + angle)) % 360;
        }

        /// <summary>
        /// Convenience method for getting the offset to a characters final position.
        /// </summary>
        Vector2 CharacterTranslation(float rotationAngle, float distance) {
            return Vector2.zero + (Vector2)(Quaternion.Euler(0f, 0f, rotationAngle) * (Vector3.right * distance));
        }
        #endregion
    }
}
