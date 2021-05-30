using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottleController : MonoBehaviour
{
	public Image mImage;
	private Material mMat;
	public Slider mSlider;
	public Vector2 BottomAndTopPos;

	// Use this for initialization
	void Start ()
	{
		mMat = mImage.material;
		mSlider.onValueChanged.AddListener (delegate { OnSliderValueChanged (); });
	}

	void OnSliderValueChanged ()
	{
		mMat.SetFloat ("_GrowUp", Mathf.Lerp (BottomAndTopPos.x, BottomAndTopPos.y, mSlider.value));
	}
}