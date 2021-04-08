using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipFire : MonoBehaviour {

	private ShipFireWeapon shipFireWeapon;

    private TimerController fireTimer;

	void Start () {
        fireTimer = new TimerController();
		shipFireWeapon = GetComponent<ShipFireWeapon>();
	}

	void Update () {
        fireTimer.Run();
		if(!PauseGame.isGamePaused)
			HandleFireWeapons ();
	}

	private void HandleFireWeapons(){
		if (HasPressedShoot() && fireTimer.GetElapsedTime() > 0.4 ) {
			shipFireWeapon.FirePrimaryWeapon();
            fireTimer.Reset();
        }
	}

	private bool HasPressedShoot(){
		return Input.GetKeyDown (KeyCode.Space);
	}
}
