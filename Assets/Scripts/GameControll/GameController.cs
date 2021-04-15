using System;
using System.IO;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour{
	
	private GameObject player;
	private ShipCollision shipCollisionStatus;

	public string actualScene;
    bool flagAuxBugFixGambiarra = true;

    void Start () {
        flagAuxBugFixGambiarra = true;
        TextEnable.Init();
        player = GameObject.FindGameObjectWithTag("Player");
        shipCollisionStatus = player.GetComponent<ShipCollision>();
        if(DataCenter.instance.numberOfLevelDeaths == 0) {
            DataCenter.instance.velMinInicial = DDAManager.instance.asteroidSpeed; //grava a velocidade inicial do nivel
            DataCenter.instance.SetTempoInicial(); //grava o tempo inicial do nível
            if (DDAManager.instance.IsAfetivo) {
                EDADatabase.instance.GetEDAFromDB(false, false); //Descarta os sinais eda lidos no questionario (calcularExcitacao=false)
            }
        }
    }


    void Update () {
		if(HasPressedExitGame())
            Application.Quit();
        if (shipCollisionStatus.isDead) {
            if(flagAuxBugFixGambiarra)
                SetTextDesExtZon();
            
            if (Input.GetKeyDown(KeyCode.Space)) {
                RestartLevel ();
            }
        }
    }

    private void SetTextDesExtZon() {
        DDAManager inst = DDAManager.instance;
        string excitacao, zona;

        if (inst.excitacao == State.PlayerState.HIGH) {
            excitacao = "h";
        }
        else if (inst.excitacao == State.PlayerState.NORMAL) {
            excitacao = "n";
        }
        else if (inst.excitacao == State.PlayerState.LOW) {
            excitacao = "l";
        }
        else {
            excitacao = "-";
        }

        if (inst.zona == State.PlayerState.HIGH) {
            zona = "h";
        }
        else if (inst.zona == State.PlayerState.NORMAL) {
            zona = "n";
        }
        else if (inst.zona == State.PlayerState.LOW) {
            zona = "l";
        }
        else {
            zona = "-";
        }
        
        if (inst.type == DDAManager.ADDTypes.Afective) {
            if (DataCenter.instance.numberOfLevelDeaths == 1) {
                NGUIDebug.Log("e" + excitacao + "z" + zona);
            }
        }
       
        TextEnable.SetHiperspaceText("Pressione Espaço");
        flagAuxBugFixGambiarra = false;

    }


    private bool HasPressedExitGame(){
		if (Input.GetKeyDown (KeyCode.Escape))
			return true;
		return false;
	}

	private void RestartLevel(){
        DDAManager.instance.deathAdjustment();
		SceneManager.LoadScene (actualScene);
	}
	
    public void SetGameWin(){
		TextEnable.EnableFaseCompleta ();
    }

	public void SetGameOver(){
		TextEnable.EnableGameLose ();
    }
 		
}	