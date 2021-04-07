using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using State;

public class DDAAply : MonoBehaviour {

	public GameObject prefab;
	public static DDAAply instance;
		
    public float asteroidSpeed = 1f; //velocidade dos asteroids

    public PlayerState excitacao; //LOW, NORMAL, HIGH, NULL(quando só desempenho)
    public PlayerState desempenho; //LOW, NORMAL, HIGH, NULL(quando só afetivo)
    public PlayerState zona; //LOW(amena), NORMAL(otima) ou HIGH(intensa)

	//private float EDA = 0; //eda values
    
	
    public bool IsAfetivo = false;
    public bool IsDesempenho = false;
    public bool IsZona = false;

    void Awake () {
		if (instance == null) {
			instance = prefab.GetComponent<DDAAply> ();
            excitacao = PlayerState.NULL;
            desempenho = PlayerState.NULL;
            zona = PlayerState.LOW;//todos começam com poucas mortes
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad (gameObject);
	}
				
    //chamada quando se passa de nível (PassLevel)
	public void BalanceAtPassLevel(){
        CalculaZona();
        if (IsDesempenho) {
            CalculaDesempenhoPassLevel();
            AjustaDesempenhoPassLevel();
        }
        else if (IsAfetivo) {
            EDAStart.instance.LerEDACalculaExcitacao(true); //le os sinais e os salva em EDAStart.instance.sinais
            //O ajuste de excitacao só é chamado dps de calcular o desempenho, e como ele é concorrente, ela é chamada só após a finalização do calculo
        }
        else if (IsZona) {
            AjustaZonaPassLevel();
        }
        else {
            Debug.Log("O jogo não está sendo balanceado");
        }
    }

    //ajusta o nível quando morre (GameController)
    public void BalanceAtDeath() {

        CalculaZona();

        float ajuste_zona = 0f;
        if (zona == PlayerState.LOW) {
            ajuste_zona = 0f;
        }
        else if (zona == PlayerState.NORMAL) {
            ajuste_zona = -0.25f;
        }
        else { //(zona == PlayerState.HIGH) 
            ajuste_zona = -0.5f;
        }
        
        if (IsDesempenho) {
            float ajuste_des = 0f;
            if (desempenho == PlayerState.LOW) {
                ajuste_des = -0.5f;
            }
            else if (desempenho == PlayerState.NORMAL) {
                ajuste_des = -0.25f;
            }
            else if (desempenho == PlayerState.HIGH) {
                ajuste_des = 0f;
            }
            else { //desempenho==NULL
                Debug.Log("Warning: É DSP e PlayerState.desempenho==NULL (BalanceAtDeath)");
                ajuste_des = -0.25f;
            }
            asteroidSpeed += (ajuste_des + ajuste_zona);
        }
        else if (IsAfetivo) {
            float ajuste_ext = 0;
            if (excitacao == PlayerState.HIGH) {
                ajuste_ext = -0.5f;
            }
            else if (excitacao == PlayerState.NORMAL) {
                ajuste_ext = -0.25f;
            }
            else if (excitacao == PlayerState.LOW) {
                ajuste_ext = 0f;
            }
            else {
                Debug.Log("Warning: É AFT e PlayerState.excitacao==NULL (BalanceAtDeath)");
                ajuste_ext = -0.25f;
            }
            asteroidSpeed += (ajuste_ext + ajuste_zona);
        }
        else if (IsZona) {
            asteroidSpeed += (ajuste_zona - 0.25f);// -0.25f para considerar que é desempenho ou excitacao normal
        }
        else {
            Debug.Log("O jogo não está sendo balanceado...");
        }
    }

    public void CalculaDesempenhoPassLevel() {
        float mortes = (float) DataCenter.instance.numberOfLevelDeaths;
        double duracao = DataCenter.instance.GetDuracao();

        double limiarMortesAltoDesempenho = 0.03619733 * Mathf.Exp(0.43041275f * asteroidSpeed); //se tiver menos mortes que isso, é alto desempenho
        double limiarDuracaoAltoDesempenho =  24.12764375 * Mathf.Exp(0.08573745f * asteroidSpeed); //se durar menos tempo que isso, é alto desemepnho
        double limiarMortesBaixoDesempenho = 0.11379691 * Mathf.Exp(0.49376684f * asteroidSpeed); //se tiver mais mortes que isso, é baixo desempenho
        double limiarDuracaoBaixoDesempenho = 35.66058598 * Mathf.Exp(0.15140602f * asteroidSpeed); //se durar mais tempo que isso, é baixo desempenho
        
        Debug.Log("CalculaDesempenho. velocidade: " + asteroidSpeed);

        if (mortes < limiarMortesAltoDesempenho && duracao < limiarDuracaoAltoDesempenho) {
            desempenho = PlayerState.HIGH;
        }
        else if(mortes > limiarMortesBaixoDesempenho && duracao > limiarDuracaoBaixoDesempenho) {
            desempenho = PlayerState.LOW;
        }
        else {
            desempenho = PlayerState.NORMAL;
        }
    }

    private void AjustaDesempenhoPassLevel() {

        NGUIDebug.Clear();

        float ajuste_des = 0f;
        if (desempenho == PlayerState.LOW) {
            Debug.Log("desL");
            NGUIDebug.Log("dl");
            ajuste_des = 0f;
        }
        else if (desempenho == PlayerState.NORMAL) {
            Debug.Log("desN");
            NGUIDebug.Log("dn");
            ajuste_des = 0.5f;
        }
        else if (desempenho == PlayerState.HIGH) {
            Debug.Log("desH");
            NGUIDebug.Log("dh");
            ajuste_des = 1f;
        }
        else { //desempenho == NULL
            Debug.Log("Warning: Desempenho == Null (AjustaDesempenho)");
            NGUIDebug.Log("d-");
            ajuste_des = 0.5f;
        }

        float ajuste_zona = 0f;
        if (zona == PlayerState.LOW) {
            Debug.Log("zonaL");
            NGUIDebug.Log("zl");
            ajuste_zona = 1f;
        }
        else if (zona == PlayerState.NORMAL) {
            Debug.Log("zonaN");
            NGUIDebug.Log("zn");
            ajuste_zona = 0.5f;
        }
        else { //(zona == PlayerState.HIGH) 
            Debug.Log("zonaH");
            NGUIDebug.Log("zh");
            ajuste_zona = 0f;
        }

        Debug.Log("Ajuste pass nivel: " + (ajuste_des + ajuste_zona) + " vel inicial: " + DataCenter.instance.velMinInicial + " vel final: " + asteroidSpeed);
        NGUIDebug.Log((ajuste_des + ajuste_zona) + "35470" + DataCenter.instance.velMinInicial + "0" + asteroidSpeed);
        asteroidSpeed += (ajuste_des + ajuste_zona);

    }

    private void AjustaZonaPassLevel() {
        NGUIDebug.Clear();
        float ajuste_zona = 0f;
        if (zona == PlayerState.LOW) {
            Debug.Log("zonaL");
            NGUIDebug.Log("zl");
            ajuste_zona = 1.5f;
        }
        else if (zona == PlayerState.NORMAL) {
            Debug.Log("zonaN");
            NGUIDebug.Log("zn");
            ajuste_zona = 1f;
        }
        else { //(zona == PlayerState.HIGH) 
            Debug.Log("zonaH");
            NGUIDebug.Log("zh");
            ajuste_zona = 0.5f;
        }
        
        NGUIDebug.Log((ajuste_zona) + "35470" + DataCenter.instance.velMinInicial + "0" + asteroidSpeed);
        asteroidSpeed += (ajuste_zona);
    }

    public void AjustaExcitacaoPassLevel() {

        NGUIDebug.Clear();

        float ajuste_ext = 0f;
        if (excitacao == PlayerState.HIGH) {
            Debug.Log("extH");
            NGUIDebug.Log("eh");
            ajuste_ext = 0f;
        }
        else if (excitacao == PlayerState.NORMAL) {
            Debug.Log("extN");
            NGUIDebug.Log("en");
            ajuste_ext = 0.5f;
        }
        else if (excitacao == PlayerState.LOW) {
            Debug.Log("extL");
            NGUIDebug.Log("el");
            ajuste_ext = 1f;
        }
        else { //excitacao == NULL
            Debug.Log("Warning: excitacao == Null (AjustaExcitacao)");
            NGUIDebug.Log("e-");
            ajuste_ext = 0.5f;
        }

        float ajuste_zona = 0f;
        if (zona == PlayerState.LOW) {
            Debug.Log("zonaL");
            NGUIDebug.Log("zl");
            ajuste_zona = 1f;
        }
        else if (zona == PlayerState.NORMAL) {
            Debug.Log("zonaN");
            NGUIDebug.Log("zn");
            ajuste_zona = 0.5f;
        }
        else { //(zona == PlayerState.HIGH) 
            Debug.Log("zonaH");
            NGUIDebug.Log("zh");
            ajuste_zona = 0f;
        }

        Debug.Log("Ajuste pass nivel: " + (ajuste_ext + ajuste_zona) + " vel inicial: " + DataCenter.instance.velMinInicial + " vel final: " + asteroidSpeed);
        NGUIDebug.Log((ajuste_ext+ajuste_zona) + "35470"+DataCenter.instance.velMinInicial+"0"+ asteroidSpeed);

        asteroidSpeed += (ajuste_ext + ajuste_zona);

    }

    public void CalculaZona() {
        int mortes = DataCenter.instance.numberOfLevelDeaths;
        double duracao = DataCenter.instance.GetDuracao();

        if (mortes < 4 && duracao < 67) {
            zona = PlayerState.LOW;
        }
        else if (mortes > 4 && duracao > 67) {
            zona = PlayerState.HIGH;
        }
        else {
            zona = PlayerState.NORMAL;
        }
    }

    public float getAsteroidSpeed() {
        return asteroidSpeed;
    }

    public string getTipoJogo() {
        if (IsDesempenho) {
            return "Desempenho";
        }
        else if (IsAfetivo) {
            return "Afetivo";
        }
        else if (IsZona) {
            return "Hibrido";
        }
        else {
            return "Erro ao selecionar o tipo de jogo";
        }
    }

    public void ChooseSensor(string sensor) {
        DataCenter.instance.setSensor(sensor);
        if (sensor == "AFT") {
            Debug.Log("É Afetivo");
            IsAfetivo = true;
            IsDesempenho = false;
            IsZona = false;
        }
        else if (sensor == "DSP") {
            Debug.Log("É Desempenho");
            IsAfetivo = false;
            IsDesempenho = true;
            IsZona = false;
        }
        else if (sensor == "ZON") {
            IsAfetivo = false;
            IsDesempenho = false;
            IsZona = true;
        }
    }

    public string getStringPlayerState(PlayerState ps) {
        if(ps == PlayerState.HIGH) {
            return "HIGH";
        }
        else if(ps == PlayerState.LOW) {
            return "LOW";
        }
        else if (ps == PlayerState.NORMAL) {
            return "NORMAL";
        }
        else {
            return "NULL";
        }
    }
}
