using UnityEngine;
using UnityEditor;

public class CartoonShaderUI : ShaderGUI
{
	bool m_UseStylizedShadow = false;
	bool m_UseRampTexture = false;
	enum ESpecType { None, Common, Stylized };
	ESpecType m_SpecType = ESpecType.None;
	
	void CollectShaderFeatureState (MaterialEditor me)
    {
		m_UseStylizedShadow = false;
		m_UseRampTexture = false;
		
		bool hasKeyword_NCE_SPECULAR = false;
		bool hasKeyword_NCE_STYLIZED_SPECULAR = false;
		
		Material mat = me.target as Material;
		string[] sks = mat.shaderKeywords;
		for (int i = 0; i < sks.Length; i++)
		{
			if (sks[i].CompareTo ("NCE_STYLIZED_SHADOW") == 0)
				m_UseStylizedShadow = true;
			if (sks[i].CompareTo ("NCE_RAMP_TEXTURE") == 0)
				m_UseRampTexture = true;
			if (sks[i].CompareTo ("NCE_SPECULAR") == 0)
				hasKeyword_NCE_SPECULAR = true;
			if (sks[i].CompareTo ("NCE_STYLIZED_SPECULAR") == 0)
				hasKeyword_NCE_STYLIZED_SPECULAR = true;
		}
		
		if (!hasKeyword_NCE_SPECULAR && !hasKeyword_NCE_STYLIZED_SPECULAR)
			m_SpecType = ESpecType.None;
		if (hasKeyword_NCE_SPECULAR && !hasKeyword_NCE_STYLIZED_SPECULAR)
			m_SpecType = ESpecType.Common;
		if (hasKeyword_NCE_SPECULAR && hasKeyword_NCE_STYLIZED_SPECULAR)
			m_SpecType = ESpecType.Stylized;
    }
	void DrawShaderFeatures (MaterialEditor me)
	{
        EditorGUIUtility.fieldWidth = 64;
        EditorGUIUtility.labelWidth = 180;
        EditorGUILayout.BeginVertical("GroupBox");
        {
			m_UseStylizedShadow = EditorGUILayout.Toggle ("StylizedShadow", m_UseStylizedShadow);
			m_UseRampTexture = EditorGUILayout.Toggle ("RampTexture", m_UseRampTexture);
			m_SpecType = (ESpecType)EditorGUILayout.EnumPopup ("Specular Type", m_SpecType);
        }
        EditorGUILayout.EndVertical();
	}
	void ChangeShaderFeature (MaterialEditor me)
	{
		Material mat = me.target as Material;
		if (m_UseStylizedShadow)
			mat.EnableKeyword ("NCE_STYLIZED_SHADOW");
		else
			mat.DisableKeyword ("NCE_STYLIZED_SHADOW");
		
		if (m_UseRampTexture)
			mat.EnableKeyword ("NCE_RAMP_TEXTURE");
		else
			mat.DisableKeyword ("NCE_RAMP_TEXTURE");
		
		if (m_SpecType == ESpecType.None)
		{
			mat.DisableKeyword ("NCE_SPECULAR");
			mat.DisableKeyword ("NCE_STYLIZED_SPECULAR");
		}
		else if (m_SpecType == ESpecType.Common)
		{
			mat.EnableKeyword ("NCE_SPECULAR");
			mat.DisableKeyword ("NCE_STYLIZED_SPECULAR");
		}
		else if (m_SpecType == ESpecType.Stylized)
		{
			mat.EnableKeyword ("NCE_SPECULAR");
			mat.EnableKeyword ("NCE_STYLIZED_SPECULAR");
		}
	}
	public override void OnGUI (MaterialEditor me, MaterialProperty[] props)
	{
		CollectShaderFeatureState (me);
		EditorGUI.BeginChangeCheck ();
		DrawShaderFeatures (me);
		if (EditorGUI.EndChangeCheck ())
			ChangeShaderFeature (me);
		
		EditorGUIUtility.fieldWidth = 64;
		EditorGUIUtility.labelWidth = 180;
		for (int i = 0; i < props.Length; i++)
		{
			MaterialProperty prop = props[i];
			me.ShaderProperty (prop, prop.displayName);
		}
	}
}
