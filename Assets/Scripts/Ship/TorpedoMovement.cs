using UnityEngine;
using System.Collections;

public class TorpedoMovement : MonoBehaviour {

    public float Speed = 20f;

	void Update () 
    {
        gameObject.transform.Translate(Speed * Time.deltaTime, 0, 0, Space.Self);
	}
}
