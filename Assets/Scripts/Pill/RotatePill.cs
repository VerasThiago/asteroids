using UnityEngine;
using System.Collections;

public class RotatePill : MonoBehaviour
{
    public int Speed;

	void Update (){
        gameObject.transform.Rotate(-Speed * Time.deltaTime, 0, 0, Space.Self);
	}
}
