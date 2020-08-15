using UnityEngine;

public class Demo : MonoBehaviour
{
	public NPRCartoonEffect[] m_Cartoons;
	
	void Start ()
	{
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
		QualitySettings.antiAliasing = 8;
		m_Cartoons = GameObject.FindObjectsOfType<NPRCartoonEffect> ();
		for (int i = 0; i < m_Cartoons.Length; i++)
			m_Cartoons[i].Initialize ();
	}
	void Update ()
	{
		for (int i = 0; i < m_Cartoons.Length; i++)
			m_Cartoons[i].UpdateSelfParameters ();
	}
	void OnGUI()
	{
		GUI.Box (new Rect (5, 5, 190, 24), "NPR Cartoon Effect Demo");
	}
}
