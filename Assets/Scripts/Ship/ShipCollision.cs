using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading;

public class ShipCollision : MonoBehaviour{
	
    private Object explosion;
    private Object bigExplosion;
    private AudioClip explosionClip;
	public bool isDead;

    void Start(){
		LoadResources ();
		isDead = false;
    }
				
	void OnTriggerEnter(Collider collider){
		if(IsFriendly(collider))
            return;
		DestroiNormalAsteroid (collider);
		PlayDeadSound ();
		DataCenter.instance.AddDeath ();
		isDead = true;
		PauseGame.isGamePaused = true;
		gameObject.SetActive (false);
    }
		
	private void LoadResources(){
		explosion = Resources.Load("Prefabs/Explosion");
		bigExplosion = Resources.Load("Prefabs/Big Explosion");
		explosionClip = Resources.Load<AudioClip>("Sounds/explosion");	
	}

	private bool IsFriendly(Collider collider){
		if (collider.CompareTag("GreenPill"))
			return true;
		return false;
	}

	private void PlayDeadSound(){
		var random = new System.Random();
		var mainAudio = GameObject.FindGameObjectWithTag("MainAudio");
		for (int i = 0; i < 5; i++){
			AudioSource explosionAudioSource = mainAudio.AddComponent<AudioSource>();
			explosionAudioSource.clip = explosionClip;
			float delay = (float) random.Next(0, 1);
			explosionAudioSource.PlayDelayed(delay);
		}
	
	}

	private void DestroiNormalAsteroid(Collider collider){
		var shipPosition = gameObject.transform.position;
		var asteroidPosition = collider.gameObject.transform.position;
		var rotation = Quaternion.identity;

		if (!collider.gameObject.tag.Equals ("AsteroidEx")) {
			Instantiate (explosion, asteroidPosition, rotation);
			Destroy (collider.gameObject);		
		}
		Instantiate(bigExplosion, shipPosition, rotation);
	}

}
