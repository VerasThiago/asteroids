using UnityEngine;
using System.Collections;

public class ShipFireWeapon : MonoBehaviour{

	private Object torpedo;

    void Start (){
        torpedo = Resources.Load("Prefabs/Photon Torpedo");
	}
	
	public void FirePrimaryWeapon(){
		
		Vector3 position = gameObject.transform.position;
		Quaternion rotation = gameObject.transform.rotation;

	    Instantiate(torpedo, position, rotation);
    }
}
	