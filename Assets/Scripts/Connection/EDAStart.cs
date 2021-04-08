using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class EDAStart : MonoBehaviour
{
    public static EDAStart instance;
    public GameObject prefab;
    public EDASignals sinais; //onde ficam os sinais lidos.

    public EDASignals signals;
    public bool isTestData;
    public EDASignals testSignals;
    public TextAsset csvTestFile;

    private int lastIDSaved;
    private int frequency = 4;
    private int readInterval = 2;

    //private readonly double tempo_inicial_bd = 1570572504.2719;
    //private double tempo_inicial_jogo;
    private TimerController timer;
    private int ultimo_id_lido;
    //private EDAProcessor edaProcessor; //Essa classe contem parte do programa do marcos usado para calcular nivel tonico
    //private double ultimo_tempo_eda_lido; //usado para gerar os graficos
    //EDATempoTonicoDTO objETT; //usado para guardar os dados dos graficos
    List<PicoEDA> picos = new List<PicoEDA>();

    public bool calculandoExcitacao = false; //é o importante o default ser false pra nao travar em UIPerguntas2 quando for por desempenho.
    //private double edaUltimoValor;
    //private double edaPenultimoValor;
    double ruido_anterior = 0; //utilizado para salvar os valores dos ruidos anteriores calculados para se usar o maior ruido achado.

    void Awake() {
        if (instance == null) {
            instance = prefab.GetComponent<EDAStart>();
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start() {
        ultimo_id_lido = 0;
        lastIDSaved = 0;
        //tempo_inicial_jogo = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;
        //tempo_inicial_jogo = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(); //também é uma opção mas tem menos precisão
        timer = new TimerController();
        timer.Reset();

        //GetReadFromJsonFiles(25.0f);

        if (isTestData && csvTestFile)
        {
            GenerateTestSignalsFromCSV();
        }
    }

    void Update() {
        timer.Run();
        if (timer.GetElapsedTime() > readInterval) {
            //    StartCoroutine(GetReadBiggerSimulation(ultimo_id_lido));

            if(isTestData && testSignals.success > 0 )
            {
                GetDataFromTestArray();
            }
            
            timer.Reset();
        }
    }

    /** 
     * Generates test signal entire data array based on csv file
     * Update will handle test data to add on correct array of signals 
     * each second.
    **/
    public void GenerateTestSignalsFromCSV()
    {
        testSignals.success = 0;
        string[] rows = csvTestFile.text.Split('\n');
        int rowCount = 0;
        foreach (string data in rows)
        {
            if (rowCount > 2 && rowCount < rows.Length)
            {
                string[] columns = data.Split(',');
                foreach (string column in columns)
                {
                    testSignals.success++;
                    if (column.Length > 0) { 
                        EDASignal newSignal = new EDASignal(0, float.Parse(column));
                        testSignals.eda.Add(newSignal);
                        // Debug.Log("Novo sinal adicionado: " + column);
                    }
                }
            }
            rowCount++;
        }
        Debug.Log("Adicionados " + testSignals.success + " dados de teste");
    }

    /** Gets last signals to simulate real time acquisition data**/
    public void GetDataFromTestArray()
    {
        int previousLastIdSaved = lastIDSaved;
        int elementsByInterval = frequency * readInterval;
        for (; lastIDSaved < (previousLastIdSaved + elementsByInterval); lastIDSaved++)
        {
            EDASignal testSignal = testSignals.eda[lastIDSaved];
            signals.eda.Add(testSignal);
            Debug.Log("EDA Salvo " + testSignal.value);
        }
    }

    public void LerEDACalculaExcitacao(bool calcularExcitacao) {
        calculandoExcitacao = true;
        StartCoroutine(GetReadBigger(calcularExcitacao));
    }

    // lê todos os números maiores que id
    //vou usar essa. o id vou salvar antes.
    //se calcularExcitacao for true, calcula a excitacao. Se for false, apenas le os dados (usado para descartar os dados do questionario)
    IEnumerator GetReadBigger(bool calcularExcitacao) {
        Debug.Log("Entrou no GetReadBigger com id==" + ultimo_id_lido);
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost/android_connect/read_bigger.php" + "?id=" + ultimo_id_lido)) {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if ((www.isNetworkError || www.isHttpError)) {
                NGUIDebug.Log("Erro de conecção");
                Debug.Log("Erro de conecção");
                Debug.Log(www.error);
            }
            else {

                string jsonString = www.downloadHandler.text;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US"); //para converter os Doubles considerando '.' e nao ','
                sinais = JsonUtility.FromJson<EDASignals>(jsonString);
                if (sinais.eda.Count > 0) {
                    ultimo_id_lido = sinais.eda[sinais.eda.Count - 1].id;
                }
                else {
                    ultimo_id_lido = 0; //caso dê clear no banco de dados no meio da partida
                }

                NGUIDebug.Clear();
                if (calcularExcitacao) {
                    NGUIDebug.Log(sinais.eda.Count + "lid");
                    Debug.Log(sinais.eda.Count + " sinais lidos");
                }
                else {
                    NGUIDebug.Log(sinais.eda.Count + "desc");
                    Debug.Log(sinais.eda.Count + " sinais descartados");
                }
                if (calcularExcitacao) {
                    picos.Clear();
                    CalculaPicos(); //pontos máximos e minimos relativos
                    NGUIDebug.Log(picos.Count + "pic");
                    Debug.Log(picos.Count + " picos achados");
                    if(picos.Count > 1) {
                        CaclulaExcitacao();
                    }
                    else if(sinais.eda.Count > 0) {
                        Debug.Log("Warning: Excitação: NORMAL (picos.Count <= 0");
                        DDAAply.instance.excitacao = State.PlayerState.NORMAL; //normal pq temos poucos sinais, entao pouca variacao
                    }
                    else {
                        Debug.Log("Warning: Excitação: NULL (sinais.eda.Count <= 0");
                        DDAAply.instance.excitacao = State.PlayerState.NULL; //null pq nao temos nenhum sinal
                    }
                    DDAAply.instance.AjustaExcitacaoPassLevel();
                }
            }
        }
        calculandoExcitacao = false;
    }

    private void CalculaPicos() {
        bool estava_subindo = false;
        bool estava_descendo = false;
        double tamanho = 0;
        double dif;
        int ignorar = 10; //numero de valores a se ignorar. o valor inicial pode ter mais relação com o questionario do que com o jogo em si. 6 foi encontrado por testes como um bom número

        sinais.eda.Sort((x, y) => x.id.CompareTo(y.id)); //se nao tiver em ordem, ordena os sinais por id

        //se tiver menos sinais do que ignorar+2, nao calcula a excitacao
        if (sinais.eda.Count < ignorar+2) {
            picos.Clear();
        }
        else {
            for (int i = 0; i < sinais.eda.Count; i++) {
                //descarta os 20 primeiros sinais
                if (i == ignorar) {
                    dif = sinais.eda[ignorar].value - sinais.eda[ignorar-1].value;
                    if (dif >= 0) {
                        estava_subindo = true;
                        tamanho = dif;
                    }
                    else {
                        estava_descendo = true;
                        tamanho = -dif;
                    }
                }
                else if (i > ignorar) {
                    dif = sinais.eda[i].value - sinais.eda[i - 1].value;
                    //se subindo agora
                    if (dif > 0) {
                        //se já estava subindo
                        if (estava_subindo) {
                            tamanho += dif;
                        }
                        //começou a subir só agora. encontrou um pico negativo em i-1
                        else if (estava_descendo) {
                            //add pico
                            picos.Add(new PicoEDA(sinais.eda[i-1],tamanho));
                            tamanho = dif;
                            estava_descendo = false;
                            estava_subindo = true;
                        }
                        else {
                            Debug.Log("Um erro está acontecendo ao calcular o EDA. (nao está detectando subida nem descida) (1)");
                        }
                    }
                    //se descendo agora
                    else {
                        //se já estava descendo
                        if (estava_descendo) {
                            tamanho -= dif;
                        }
                        //comçou a descer só agora. encontrou um pico positivo em i-1
                        else if (estava_subindo) {
                            //add pico;
                            picos.Add(new PicoEDA(sinais.eda[i - 1], tamanho));
                            tamanho = -dif;
                            estava_subindo = false;
                            estava_descendo = true;
                        }
                        else {
                            Debug.Log("Um erro está acontecendo ao calcular o EDA. (nao está detectando subida nem descida) (2)");
                        }
                    }
                }
            }
        }
    }

    private void CaclulaExcitacao() {
        double edaInicialMedia;
        double edaFinalMedia;
        int s = picos.Count;
        if (s > 3) {
            edaInicialMedia = (picos[0].value + picos[1].value + picos[2].value + picos[3].value) /4;
            edaFinalMedia = (picos[s-1].value + picos[s-2].value + picos[s-3].value + picos[s-4].value)/4;
        }
        else {
            edaInicialMedia = picos[0].value;
            edaFinalMedia = picos[s - 1].value;
        }
        picos.Sort((x, y) => x.size.CompareTo(y.size)); //ordena a lista pra encontrar o ruido
        double ruido = picos[(int)(picos.Count / 2)].size; //o ruido é considerado a amplitude mediana entre dois picos
        //usamos o maior ruido calculado como ruido ja que em alguns niveis onde a excitacao nao variar muito, o ruido fiquei mais baixo do que em niveis mais agitados. e queremos detectar os niveis nao agitados como excitacao NORMAL
        //mas dps descartei essa mudança pq nao expliquei ela no tcc e sei la, pode dar algum erro se algum nivel o ruido for mt alto e dps todo mundo for considerado normal pra sempre.
        //if (ruido_anterior > ruido) {
            //ruido = ruido_anterior;
        //}

        int escala = 2; //quantas vezes uma mudança tem que ter em relação ao ruido para se considerar se o jogador se excitou ou não

        Debug.Log("edaInicialMedia: " + edaInicialMedia);
        Debug.Log("edaFinalMedia: " + edaFinalMedia);
        Debug.Log("ruido: " + ruido);
        //subiu
        if (edaFinalMedia - edaInicialMedia > escala * ruido) {
            Debug.Log("Excitação: HIGH");
            DDAAply.instance.excitacao = State.PlayerState.HIGH;
        }
        //desceu
        else if(edaFinalMedia - edaInicialMedia < -1 * escala * ruido) {
            Debug.Log("Excitação: LOW");
            DDAAply.instance.excitacao = State.PlayerState.LOW;
        }
        else {
            Debug.Log("Excitação: NORMAL");
            DDAAply.instance.excitacao = State.PlayerState.NORMAL;
        }
    }

    //utilizada porque tava dando bug quando se zerava o banco para sincronizar com o jogo e o id nao atualizava
    public void zerarId() {
        ultimo_id_lido = 0;
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
    }

    // lê todos os dados na tabela eda do servidor
    IEnumerator GetReadAll() {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost/android_connect/read_all.php")) {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if ((www.isNetworkError || www.isHttpError)) {
                Debug.Log(www.error);
            }
            else {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US"); //para converter os Doubles considerando '.' e nao ','
                string jsonString = www.downloadHandler.text;
                sinais = JsonUtility.FromJson<EDASignals>(jsonString);
            }

        }
    }

    // lê todos os números maiores que id e menores do que determinado tempo passado em relação a tempo_inicial_bd
    // usado pra simular quando nao se esta pegando os dados em tempo real
    IEnumerator GetReadBiggerSimulation(int id) {
        double tempo_agora = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;
        double tempo_aux = (tempo_agora - tempo_inicial_jogo) + tempo_inicial_bd;
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost/android_connect/read_between.php" + "?id=" + id + "&tempo=" + tempo_aux)) {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if ((www.isNetworkError || www.isHttpError)) {
                Debug.Log(www.error);
            }
            else {
                string jsonString = www.downloadHandler.text;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US"); //para converter os Doubles considerando '.' e nao ','
                sinais = JsonUtility.FromJson<EDASignals>(jsonString);
                Debug.Log(jsonString);
                //ultimo_id_lido = sinais.eda[sinais.eda.Length-1].id;
                ultimo_id_lido = sinais.eda[sinais.eda.Count - 1].id;
                print("GetTonicLevel: " + edaProcessor.GetTonicLevel(sinais.eda));
                print("GetPhasicLevel: " + edaProcessor.GetPhasicLevel(sinais.eda));
            }
        }
    }
    
    // lê todos os números maiores que id e menores do que determinado tempo passado como parametro
    IEnumerator GetReadBetween(int id, float tempo) {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost/android_connect/read_between.php" + "?id=" + id + "&tempo=" + tempo)) {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if ((www.isNetworkError || www.isHttpError)) {
                Debug.Log(www.error);
            }
            else {
                string jsonString = www.downloadHandler.text;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US"); //para converter os Doubles considerando '.' e nao ','
                sinais = JsonUtility.FromJson<EDASignals>(jsonString);
                //print(">>"+jsonString);
                ultimo_id_lido = sinais.eda[sinais.eda.Count - 1].id;
            }
        }
    }*/

}
