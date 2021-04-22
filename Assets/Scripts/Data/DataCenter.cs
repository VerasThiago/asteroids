using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;
using System;
using State;

// Possui uma unica instancia. 
// É usada para se passar dados dados entre diferentes cenas do jogo.
// Está atrelada ao prefab "DataReceiver"
// No final escreve os dados no JSON representado por pelo objeto df (DataFile)
public class DataCenter : MonoBehaviour {

    public GameObject prefab;

    static public DataCenter instance = null;
    public static int currentLevel;
    public string nomeCompleto;
    public int numberOfLevelDeaths;
    public double initialLevelTime;
    public double finalLevelTime;
    public float velMinInicial;
    DataFile df;
    DataFileEDA dfEda;

    void Start() {
        if (instance == null)
        {
            instance = this;
            Debug.Log(instance);
            df = new DataFile();
            dfEda = new DataFileEDA();
            currentLevel = 1;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }


    void Update() {
        double agora = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;

        if (Input.GetKey(KeyCode.Q)){
            if (Input.GetKeyDown(KeyCode.W)) {
                df.addFlagEmpatica(agora);
            }
        }
        /*if (Input.GetKeyDown(KeyCode.UpArrow)) {
            df.addApertouUp(agora);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow)) {
            df.addSoltouUp(agora);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            df.addApertouDown(agora);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow)) {
            df.addSoltouDown(agora);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            df.addApertouLeft(agora);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            df.addSoltouLeft(agora);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            df.addApertouRight(agora);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow)) {
            df.addSoltouRight(agora);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            df.addTiro(agora);
        }*/
	}

	public void AddDeath(){
		numberOfLevelDeaths++;
        df.addMorte(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds);
	}

    public void AddSpeedAdjustment(float value)
    {
        df.addValueAdjustment(value);
        df.addAdjustment(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds);
    }

    public void AddFirstEdaTime()
    {
        df.addFirstEda(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds);
    }

    public void AddLevelInfoToDataFile() {
        int AsteroidCount = GameObject.FindGameObjectWithTag("LevelController").GetComponent<CreateAsteroids>().GetAsteroidCount();
        float minSpeed = GameObject.FindGameObjectWithTag("LevelController").GetComponent<CreateAsteroids>().GetMinSpeed();
        float maxSpeed = GameObject.FindGameObjectWithTag("LevelController").GetComponent<CreateAsteroids>().GetMaxSpeed();
        df.AddLevelInfoToDataFileLevel (AsteroidCount, minSpeed, maxSpeed, velMinInicial);
    }

    public void AddPerguntasToDataFile(string respostaDificuldade, string respostaTedio, string respostaFrustracao, string respostaDiversao, string respostaInputText) {
        df.AddPerguntasToDataFileLevel(respostaDificuldade, respostaTedio, respostaFrustracao, respostaDiversao, respostaInputText);
    }

    public void Write() {
        dfEda.addSignalsOnFile(EDADatabase.instance.allSignals.eda, DDAManager.instance.changes);
        string jsonstringeda = JsonUtility.ToJson(dfEda, true);
        File.WriteAllText("sinais_" + nomeCompleto + ".json", jsonstringeda);

        string jsonstring = JsonUtility.ToJson(df, true);
        File.WriteAllText("Output "+nomeCompleto+".json", jsonstring);
    }

    public void SetNomeJogador(string nome_str, string sobrenome_str) {
        nomeCompleto = nome_str + " " + sobrenome_str;
        df.nomeCompleto = nomeCompleto;
    }

    public void SetTempoInicial() {
        initialLevelTime = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;
        df.SetTempoInicial(initialLevelTime);
    }

    public void SetTempoFinal() {
        finalLevelTime = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;
        df.SetTempoFinal(finalLevelTime);
    }

    public void SetVenceu(bool venceu) {
        df.SetVenceu(venceu);
    }

    public int GetCurrentLevel() {
        return currentLevel;
    }

    public double GetDuracao() {
        return (finalLevelTime - initialLevelTime);
    }

    public void AddLevel() {
        currentLevel++;
    }

    public void AddLevelToJson() {
        df.AddLevelToJson();
    }

    public void resetDeath() {
        numberOfLevelDeaths = 0;
    }

    internal void setSensor(string sensor) {
        df.sensor = sensor;
    }
}
