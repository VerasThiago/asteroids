using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour {


	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			Application.Quit ();
		}	
	}
}
