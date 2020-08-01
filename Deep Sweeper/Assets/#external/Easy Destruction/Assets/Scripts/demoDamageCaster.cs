using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Noir Project/Easy Destruction/Demo Damage Caster")]
public class demoDamageCaster : MonoBehaviour {

    public Transform MeshToDamage = null;
    public TextMesh scoreText = null;
    public float damageAmount = 25.0f;

    void OnMouseDown()
    {
        if (MeshToDamage != null)
        {
            if (MeshToDamage.GetComponent<EasyDestruction>() != null)
            {
                MeshToDamage.GetComponent<EasyDestruction>().castDamage(damageAmount);
            }
        }
    }

	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
		if (MeshToDamage != null)
        {
            if (MeshToDamage.GetComponent<EasyDestruction>() != null)
            {
                 if (scoreText != null)
                 {
                     scoreText.text = "Box Health: " + MeshToDamage.GetComponent<EasyDestruction>().getHealth();
                 }
            }
        }
	}
}
