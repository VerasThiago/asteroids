using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// mostrar imagens na tela
public class StaticManager : MonoBehaviour {

    public GameObject seta;
    public GameObject cameraIcone;
    private bool mostrarSeta = false;

	void Awake () {
		TextEnable.Init ();
		TextEnable.EnableDistance ();
    }

    private void Start()
    {

        cameraIcone.SetActive(false);
        if (DataCenter.instance.GetCurrentLevel() == 1) {
            mostrarSeta = true;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q)) {
            if(Input.GetKey(KeyCode.W)) {
                cameraIcone.SetActive(true);
                EDADatabase.instance.GetEDAFromDB(false, false); //Descarta os ultimos sinais lidos
                EDADatabase.instance.ClearLastID();
            }
            else {
                cameraIcone.SetActive(false);
            }
        }
        if (mostrarSeta) {
            if (Input.GetKey("up"))
            {
                mostrarSeta = false;
            }
        }
        else {
            seta.SetActive(false);
        }
        
    }
    //deixar sempre desligado, se for level 1 ligar no awake

}
