using UnityEngine;
using System.Collections;

public class CreateAsteroids : MonoBehaviour {

    private int AsteroidCount = 1000;

    private readonly float minSize = 1.75f;
    private readonly float maxSize = 2.00f;

    private float minSpeed = 1f;
    private float maxSpeed = 2f;

    public static float minX = 20f;
    public static float maxX = 600f;
    public static float minY = -100f;
    public static float maxY = 100f;

    public bool isNormal;

    public GameObject asteroidPrefab;

    void Start() {
        
        AsteroidCount = 1000;
        minSpeed = DDAManager.instance.getAsteroidSpeed();
        if(minSpeed < 0f) {
            minSpeed = 0f;
        }
        maxSpeed = minSpeed + 1f;
        for (int i = 0; i < AsteroidCount; i++) {

            GameObject asteroid = (GameObject)Instantiate(asteroidPrefab, StartPosition(), Quaternion.Euler(0, 0, 0));

            float scale = Random.Range(minSize, maxSize);
            if (scale >= 5.0f && !asteroid.GetComponent<AsteroidType>().explosive)
                asteroid.GetComponent<AsteroidType>().indestructible = true;

            asteroid.transform.localScale = new Vector3(scale, scale, scale);

            AsteroidMovement movement = GameObject.FindObjectOfType<AsteroidMovement>();
            movement.Direction = AsteroidDirection();
            movement.setSpeed(Random.Range(minSpeed, maxSpeed));
        }
    }


    private Vector3 AsteroidDirection() {
        float angle = Random.Range(0, 360);
        float mx = Mathf.Cos(Mathf.Deg2Rad * angle);
        float my = Mathf.Sin(Mathf.Deg2Rad * angle);
        float mz = 0f;

        return new Vector3(mx, my, mz);
    }

    static public Vector3 StartPosition() {
        var x = Random.Range(minX, maxX);
        var y = Random.Range(minY, maxY);
        var z = 0f;
        return new Vector3(x, y, z);
    }

    public int GetAsteroidCount() {
        return AsteroidCount;
    }

    public float GetMaxSpeed() {
        return maxSpeed;
    }

    public float GetMinSpeed() {
        return minSpeed;
    }

}
