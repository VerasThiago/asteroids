using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	private ShipCollision shipCollisionStatus;
	private ShipMove shipMove;
	private ShipFire shipFire;
	private int distance;
	private bool hasArrivedHiperspace = false;
	private const int ENDGAMEPOINT = 200;
	public bool hasWon = false;

	void Start () {		
		LoadObjects ();
	}

	void Update(){
		UpdateWinnigDistance ();
		ArrivedHiperspace ();
	}

	private void UpdateWinnigDistance(){	
		if (!hasArrivedHiperspace) {
			distance = ENDGAMEPOINT - (int)gameObject.transform.position.x;
			if(!shipCollisionStatus.isDead)
				TextEnable.UpdateDistanceText (distance.ToString ());
		} else
			TextEnable.SetHiperspaceText ("Pressione espaço");
	}
	private void ArrivedHiperspace(){
		if (distance <= 0) {
			hasArrivedHiperspace = true;
			hasWon = true;
		}
	}		

	private void LoadObjects(){		
		shipCollisionStatus = gameObject.GetComponent<ShipCollision> ();
		shipFire = gameObject.GetComponent<ShipFire> ();
		shipMove = gameObject.GetComponent<ShipMove> ();
	}
}