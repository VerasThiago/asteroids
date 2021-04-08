using UnityEngine;
using System.Collections;

public class ShieldCollision : MonoBehaviour{

	 private Object _explosion;

    void Start(){
        _explosion = Resources.Load("Prefabs/Explosion");
    }

    void OnTriggerEnter(Collider collider){
		
		if (IsPlayer())
            return;

        if (collider.gameObject.tag.Equals("Asteroid")){

			Vector3 asteroidPosition = GetComponent<Collider>().gameObject.transform.position;
			Quaternion rotation = Quaternion.identity;
            Instantiate(_explosion, asteroidPosition, rotation);

            Destroy(collider.gameObject);
			DisableGameObjectRenderAndCollider ();
			PlayCollisionSound ();

		}

        else if (collider.gameObject.tag.Equals("ShieldA")){

			DisableGameObjectRenderAndCollider ();
			PlayCollisionSound ();

			var shield = collider.gameObject;
			shield.GetComponent<MeshRenderer>().enabled = false;
            shield.GetComponent<SphereCollider>().enabled = false;
            shield.GetComponent<AudioSource>().Play();
        }
        else if (collider.gameObject.tag.Equals("AsteroidEx")){
			DisableGameObjectRenderAndCollider ();
			PlayCollisionSound ();
			collider.gameObject.GetComponent<SphereCollider>().enabled = false;

        }
	}

	private void DisableGameObjectRenderAndCollider(){

		gameObject.GetComponent<MeshRenderer>().enabled = false;
		gameObject.GetComponent<SphereCollider>().enabled = false;

	}

	private void PlayCollisionSound(){
		gameObject.GetComponent<AudioSource>().Play();
	}

	private bool IsPlayer(){
		if (GetComponent<Collider>().gameObject.tag.Equals("Player"))
			return true;
		return false;
	}

}