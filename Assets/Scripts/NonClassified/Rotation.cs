using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {

    public int Speed = 10;

	void Update (){
		RotateGameObject ();
	}

	private void RotateGameObject(){
		var delta = Time.deltaTime;
		var angle = Speed * delta;
		gameObject.transform.Rotate(angle, angle, angle);
	}
}
