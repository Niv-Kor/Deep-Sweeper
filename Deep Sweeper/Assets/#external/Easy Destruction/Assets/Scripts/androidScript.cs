using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("Noir Project/Easy Destruction/Easy Destruction Android Demo Looper")]
public class androidScript : MonoBehaviour {

    public bool isActive = true;
    public float time = 12.0f;
    public string level = "DemoBoxSceneAndroid";

    public IEnumerator looper()
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(level);

    }
	// Use this for initialization
	void Start () 
    {
        if (isActive)
            StartCoroutine(looper());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
