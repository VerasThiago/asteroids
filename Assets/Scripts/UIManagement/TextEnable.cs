using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEnable : MonoBehaviour {

	static private Text countdownText;
	static private Text gameWinText;
	static private Text gameLoseText;
	static private Text faseCompleta;
	static private Text distanceText;
	static private Text timeRemaining;

	static public void Init () {		
		countdownText = GameObject.FindGameObjectWithTag("Countdown").GetComponent<Text>();
		gameWinText = GameObject.FindGameObjectWithTag("GameWin").GetComponent<Text>();
		gameLoseText = GameObject.FindGameObjectWithTag("GameLose").GetComponent<Text>();
		faseCompleta = GameObject.FindGameObjectWithTag("Completa").GetComponent<Text>();
		distanceText = GameObject.FindGameObjectWithTag("Distancia").GetComponent<Text>();
		timeRemaining = GameObject.FindGameObjectWithTag ("TimeRemaining").GetComponent<Text> ();
		DisableAll ();
	}

	static public void UpdateTimeRemaining(int time){
		timeRemaining.text = "a sessao sera encerrada em: " +time + "s";
	}

	static public void UpdateDistanceText(string newDistance){
		distanceText.text = newDistance;
	}

	static public void EnableTimeRemaining(){
		timeRemaining.enabled = true;
	}

	static public void SetHiperspaceText(string hiperspace){
		distanceText.text = hiperspace;	
	}


	static public void EnableDistance(){
		distanceText.enabled = true;
	}

	static public void EnableGameWin(){
		gameWinText.enabled = true; 
	}

	static public void EnableGameLose(){
		gameLoseText.enabled = true;
	}

	static public void EnableFaseCompleta(){
		faseCompleta.enabled = true;
	}

	static public void EnableCountDown(){
		countdownText.enabled = true;
	}

	static private void DisableAll(){
		countdownText.enabled = false;
		gameWinText.enabled = false;
		gameLoseText.enabled = false;
		faseCompleta.enabled = false;
		timeRemaining.enabled = false;
	}

}
