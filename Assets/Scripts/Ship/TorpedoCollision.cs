using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class TorpedoCollision : MonoBehaviour{
	
    private Object _explosion;
    private Object _explosionEx;
    private AudioClip _explosionClip;

    void Start(){
		LoadResources ();
    }

    void OnTriggerEnter(Collider collider){
		
        if (collider.gameObject.tag.Equals("Asteroid")){
			
            var asteroid = collider.gameObject;
            var destroy = !asteroid.GetComponentInChildren<AsteroidType>().indestructible;
            var asteroidPosition = asteroid.transform.position;
            var rotation = Quaternion.identity;
            var asteroidMovement = asteroid.GetComponentInChildren<AsteroidMovement>();
            var direction = asteroidMovement.Direction;

			if (destroy){ 
                if (asteroid.GetComponent<AsteroidType>().explosive){
                    Instantiate(_explosionEx, asteroidPosition, rotation);
                }
                else{
                    Instantiate(_explosion, asteroidPosition, rotation);
                }
                var camera = GameObject.FindGameObjectWithTag("MainAudio");
                var explosionAudio = camera.AddComponent<AudioSource>();

                explosionAudio.clip = _explosionClip;
                explosionAudio.Play();

                var size = asteroid.transform.localScale.x;

				if (size > 2.0f && !asteroid.GetComponent<AsteroidType>().explosive)
                    CreateChildAsteroids(asteroidPosition, direction, size);

                Destroy(asteroid);
            }
            Destroy(gameObject);

        }
		else if(collider.gameObject.tag.Equals("ShieldA")){
            var shield = collider.gameObject;

            shield.GetComponent<MeshRenderer>().enabled = false;
            shield.GetComponent<SphereCollider>().enabled = false;
            shield.GetComponent<AudioSource>().Play();
            Destroy(gameObject);
        }
    }

    private void CreateChildAsteroids(Vector3 position, Vector3 direction, float size){
        CreateChildAsteroid(position, direction, size);
        CreateChildAsteroid(position, direction, size);
    }

    private void CreateChildAsteroid(Vector3 position, Vector3 direction, float size){
		var gameObject = CreateAsteroid (position,size);
		var movement = gameObject.GetComponentInChildren<AsteroidMovement>();
		movement.Direction = CalculateChildDirection(direction);
		ReduceAsteroid (size);

    }

	private void ReduceAsteroid(float size){
		var childSize = size / 2f;
		gameObject.transform.localScale = new Vector3(childSize, childSize, childSize);
	
	}

	private GameObject CreateAsteroid(Vector3 position,float size){
		var asteroid = Resources.Load("Prefabs/AsteroidNormal");
		var rotation = new Quaternion(0f, 0f, 0f, 0f);
		var gameObject = (GameObject)Instantiate(asteroid, position, rotation);
		return gameObject;
	}

	private Vector3 CalculateChildDirection(Vector3 direction){
		var x = Math.Abs(direction.x);
		var y = Math.Abs(direction.y);
		var mx = Random.Range(-x, x);
		var my = Random.Range(-y, y);
		var mz = 0f;
		return new Vector3(mx, my, mz);
	}

	private void LoadResources(){
		_explosion = Resources.Load("Prefabs/Explosion");
		_explosionEx = Resources.Load("Prefabs/Asteroid Explosion");
		_explosionClip = Resources.Load<AudioClip>("Sounds/explosion");	
	}

}
