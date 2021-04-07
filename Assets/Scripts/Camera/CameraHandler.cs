using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour {

	private CameraChase cameraChase;

	void Start () {
		
		var camera = GameObject.FindGameObjectWithTag("MainCamera");
		cameraChase = camera.GetComponentInChildren<CameraChase>();
	}
	
	void Update () {
		cameraChase.UpdateCamera ();
		if (HasPressedIncrease ())
			CameraIncreaseView ();
		else if (HasPressedDecrease ())
			CameraDecreaseView ();
	}

	private bool HasPressedIncrease(){
		if (Input.GetKeyDown (KeyCode.LeftAlt))
			return true;
		return false;
	}

	private void CameraIncreaseView(){	
		cameraChase.IncreaseView();
	}

	private bool HasPressedDecrease(){
		if (Input.GetKeyDown (KeyCode.RightAlt))
			return true;
		return false;	
	}

	private void CameraDecreaseView(){
		cameraChase.DecreaseView();            	
	}
}
