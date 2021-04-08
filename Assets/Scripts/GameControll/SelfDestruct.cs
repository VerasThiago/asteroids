using System;
using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {

    DateTime selfDestructTime;
    public int LifeMilliseconds = 5000;

	void Start () {
        selfDestructTime = DateTime.Now.AddMilliseconds(LifeMilliseconds);
	}
	
	void Update () {
        if (DateTime.Now > selfDestructTime)
            Destroy(gameObject);
	}
}
