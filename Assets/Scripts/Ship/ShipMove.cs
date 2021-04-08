using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMove : MonoBehaviour {

	private ShipMovement shipMovement;
	private const int ENDGAMEPOSITION = 705;
	private bool hasWin = false;

	void Start () {
		shipMovement = GetComponent<ShipMovement> ();
	}
	
	void Update () {
		if(!PauseGame.isGamePaused)
			HandleMovement ();	
	}

	private void HandleMovement(){
		var ship = GameObject.FindGameObjectWithTag("Player");

		if (ship.transform.position.x > ENDGAMEPOSITION) {
			if (!hasWin) {
				TextEnable.EnableFaseCompleta ();
				hasWin = true;
			}
		}

		if (Input.GetKey(KeyCode.LeftArrow))
			shipMovement.RotateLeft();
		else if (Input.GetKey(KeyCode.RightArrow))
			shipMovement.RotateRight();
		if (Input.GetKey(KeyCode.UpArrow) && (ship.transform.position.x>ENDGAMEPOSITION) && (shipMovement.HasWarp))
			shipMovement.Warp();
		else if (Input.GetKey(KeyCode.UpArrow))
			shipMovement.MoveForward();
		else if (Input.GetKey(KeyCode.DownArrow))
			shipMovement.MoveBackward();
		else
			shipMovement.Decelerate();
		if (!Input.GetKey(KeyCode.UpArrow)|| !(ship.transform.position.x > ENDGAMEPOSITION) )
			shipMovement.DeWarp();
	}

}
