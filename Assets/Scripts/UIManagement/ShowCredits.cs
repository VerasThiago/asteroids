using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCredits : MonoBehaviour {

	private GameObject credits;
	private Text copyright;
	private bool canCount;


	void Start () {
		SetUpVariables ();
	}

	private void SetUpVariables(){
		copyright = GameObject.FindGameObjectWithTag("Copyright").GetComponent<Text>();
		credits = GameObject.FindGameObjectWithTag("Credits");
		credits.SetActive(false);
		copyright.enabled = false;
	}

	public void EnableCredits(){
		credits.SetActive(true);
		copyright.enabled = true;
	}

}
