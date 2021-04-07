using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController{

	private float elapsedTimeSeconds;

	public TimerController(){
		elapsedTimeSeconds = 0;
	}

	public TimerController(float initialTime){
		elapsedTimeSeconds = initialTime;
	}

	public void Run(){
		elapsedTimeSeconds += Time.deltaTime;
	}

	public float GetElapsedTime(){
		return elapsedTimeSeconds;
	}

	public void Reset(){
		elapsedTimeSeconds = 0;
	}

}
