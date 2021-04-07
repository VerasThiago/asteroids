using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour {

	static public bool isGamePaused;

	void Start(){
		isGamePaused = false;
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.G))
			isGamePaused = !isGamePaused;
	}

}
