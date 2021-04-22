using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class EDADatabase : MonoBehaviour
{
    public static EDADatabase instance;
    public GameObject prefab;

    public EDASignals signals;
    public EDASignals allSignals;
    public EDASignals testSignals;

    private int lastIDSaved;
    private int frequency = 4;
    private int readInterval = 2;
    private bool savedFirst = false;

    //private double tempo_inicial_jogo;
    private TimerController timer;

    //EDATempoTonicoDTO objETT; //usado para guardar os dados dos graficos
    public bool isAdjusting = false; // Game is not adjusting on initial state

    void Awake() {
        if (instance == null) {
            instance = prefab.GetComponent<EDADatabase>();
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start() {
        lastIDSaved = 0;
        //tempo_inicial_jogo = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;
        timer = new TimerController();
        timer.Reset();

        //GetReadFromJsonFiles(25.0f);
    }

    void Update() {
        timer.Run();
        if (timer.GetElapsedTime() > readInterval) {
            if (DDAManager.instance.type == DDAManager.ADDTypes.GSR ||
                DDAManager.instance.type == DDAManager.ADDTypes.DSA) {
                GetEDAFromDB(true, true);
            }
            
            timer.Reset();
        }
    }

    public void GetEDAFromDB(bool needAdjustment, bool isLimited)
    {
        isAdjusting = true;
        StartCoroutine(GetEDAByGreaterID(needAdjustment, isLimited));
    }

    /** Method to get EDA values from DB with ID grater than argument
     * Can have a limit number of values to retrieve if isLimited is true.
     * If need adjusment is true call DDA adjusment steps based on DDA type.
     * **/
    IEnumerator GetEDAByGreaterID(bool needAdjustment, bool isLimited)
    {
        /*
        if (needAdjustment)
        {
            Debug.Log("Trying to get EDA with id greater than " + lastIDSaved);
        }
        else
        {
            Debug.Log("Discarding EDA with id greater then " + lastIDSaved);
        }*/

        String url = "http://localhost/android_connect/read_bigger.php" + "?id=" + lastIDSaved;

        if (isLimited)
        {
            url += "&limit=" + (frequency * readInterval);
        }

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if ((www.isNetworkError || www.isHttpError))
            {
                NGUIDebug.Log("Erro de conexão");
                Debug.Log("Could not connect to DB.");
                Debug.Log(www.error);
            }
            else
            {
                string jsonString = www.downloadHandler.text;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US"); //para converter os Doubles considerando '.' e nao ','
                signals = JsonUtility.FromJson<EDASignals>(jsonString);

                for (int i = 0; i < signals.eda.Count; i++)
                {
                    signals.eda[i].time = double.Parse(signals.eda[i].stringtime);
                    //Debug.Log(signals.eda[i].time);
                }

                if (signals.eda.Count > 0)
                {
                    lastIDSaved = signals.eda[signals.eda.Count - 1].id; // Save ID of last EDA
                }
                else
                {
                    lastIDSaved = 0; // If DB is cleared in the middle of game
                }

                NGUIDebug.Clear();
                if (needAdjustment)
                {
                    if(!savedFirst)
                    {
                        DataCenter.instance.AddFirstEdaTime();
                    }
                    allSignals.eda.AddRange(signals.eda);
                    NGUIDebug.Log(signals.eda.Count + " new EDA");
                    Debug.Log(signals.eda.Count + " new EDAs received");
                    DDAManager.instance.edaRequestAdjusment();
                }
                else
                {
                    NGUIDebug.Log(signals.eda.Count + " new EDA wo Adjust");
                    Debug.Log(signals.eda.Count + " new EDAs without adjustment");
                }
            }
        }
        isAdjusting = false;
    }

    /** Clear Last ID saved to successful sync with database **/
    public void ClearLastID()
    {
        lastIDSaved = 0;
    }

    /*
    // le todos os arquivos StreamingAssets/input_eda_tempo/EDA_tempo_<numero>.json e faz uma simulação em que:
    // - se le os dados em um intervalo de tempo determinado por tempo_buffer 
    // - calcula um nivel tonico e um nivel fasico para esse internvalo, 
    // - salva a saida em "ETT_<numero>.json
    private void GetReadFromJsonFiles(float tempo_buffer) {

        edaProcessor = new EDAProcessor();
        string path = Application.streamingAssetsPath + "/input_eda_tempo";
        for (int i_voluntario = 1; i_voluntario < 19; i_voluntario++) {
            ultimo_id_lido = 0;
            string jsonString = File.ReadAllText(path + "/EDA_tempo_" + i_voluntario.ToString() + ".json");
            objETT = JsonUtility.FromJson<EDATempoTonicoDTO>(jsonString); //entrada do eda e saida do tonic e phasic
            ultimo_tempo_eda_lido = objETT.tempoEda[0];
            List<EDASignal> sinais_buffer = new List<EDASignal>();
            edaProcessor.Reset();
            int id_aux = 0;
            for (int i = 0; i < objETT.tempoEda.Count; i++) {
                if (objETT.tempoEda[i] - ultimo_tempo_eda_lido < tempo_buffer) {
                    sinais_buffer.Add(new EDASignal(id_aux, objETT.tempoEda[i], objETT.eda[i], 0));
                    id_aux++;
                }
                else {

                    objETT.tempoTonicLevel.Add(objETT.tempoEda[i]);
                    int tonic = edaProcessor.GetTonicLevel(sinais_buffer);
                    objETT.tonicLevel.Add(tonic);
                    objETT.phasicLevel.Add(edaProcessor.GetPhasicLevel(sinais_buffer));
                    sinais_buffer = null;
                    sinais_buffer = new List<EDASignal>();
                    ultimo_tempo_eda_lido = objETT.tempoEda[i];

                }
            }
            jsonString = JsonUtility.ToJson(objETT, true);
            File.WriteAllText(path + "/ETT_" + i_voluntario + ".json", jsonString);
        }
    }*/
}
