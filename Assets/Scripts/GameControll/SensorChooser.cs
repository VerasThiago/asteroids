using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensorChooser : MonoBehaviour {

	public Dropdown sensor;
	public List<string> Options =  new List<string>();

	void Start () {
		sensor.ClearOptions ();
		sensor.AddOptions (Options);
	}

	public void SaveSensor(){
        DDAAply.instance.ChooseSensor(sensor.options[sensor.value].text);
	}
	
}
