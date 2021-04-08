using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {
	
    private float asteoidStartTime;
    public float timeNow;
    public float ExpirationTime;

	void Start () {
		asteoidStartTime = Time.fixedTime;
        ExpirationTime = 2;
	}
	
	void Update () {
        timeNow = Time.fixedTime;
		DestroiAsteroid ();
	}

	private void DestroiAsteroid(){
		if (timeNow > asteoidStartTime + ExpirationTime)
			Destroy(gameObject);
	}

}
