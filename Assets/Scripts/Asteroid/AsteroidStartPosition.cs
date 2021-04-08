using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidStartPosition{

	public static float mimX = 50f;
	public static float maxX = 600f;
	public static float minY = -100f;
	public static float maxY = 100f;

	static public Vector3 StartPosition(){
		var x = Random.Range(mimX, maxX);
		var y = Random.Range(minY, maxY);
		var z = 0f;
		return new Vector3(x, y, z);
	}
}
