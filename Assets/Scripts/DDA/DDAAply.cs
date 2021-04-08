using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using State;

public class DDAAply : MonoBehaviour
{

    public GameObject prefab;
    public static DDAAply instance;

    public float asteroidSpeed = 1f; //velocidade dos asteroids

    public PlayerState excitacao; //LOW, NORMAL, HIGH, NULL(quando só desempenho)
    public PlayerState zona; //LOW(amena), NORMAL(otima) ou HIGH(intensa)

    public PlayerState arousal; // Player arousal based on EDA interpretation

    //private float EDA = 0; //eda values

    public enum ADDTypes { None, Afective, GSR, DSA };
    public ADDTypes type = ADDTypes.None;

    public bool IsAfetivo = false;
    public bool IsZona = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = prefab.GetComponent<DDAAply>();
            excitacao = PlayerState.NULL;
            zona = PlayerState.LOW; // Starting player zone with low deaths
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void SetDDAType(string typeText)
    {
        DataCenter.instance.setSensor(typeText);
        if (typeText == "Nenhum")
        {
            Debug.Log("Selecionado nenhum tipo de ADD");
            type = ADDTypes.None;
            IsAfetivo = true;
            IsZona = false;
        }
        else if (typeText == "Afetivo")
        {
            Debug.Log("Selecionado tipo de ADD afetivo (legado)");
            type = ADDTypes.Afective;
            IsAfetivo = true;
            IsZona = false;
        }
        else if (typeText == "Tempo Real GSR")
        {
            Debug.Log("Selecionado tipo de ADD algoritmo GSR");
            IsAfetivo = false;
            IsZona = false;
        }
        else if (typeText == "Tempo Real DSA")
        {
            Debug.Log("Selecionado tipo de ADD algoritmo DSA");
            IsAfetivo = false;
            IsZona = false;
        }
    }

    //chamada quando se passa de nível (PassLevel)
    public void BalanceAtPassLevel()
    {
        CalculaZona();

        if (IsAfetivo)
        {
            EDAStart.instance.LerEDACalculaExcitacao(true); //le os sinais e os salva em EDAStart.instance.sinais
            //O ajuste de excitacao só é chamado dps de calcular o desempenho, e como ele é concorrente, ela é chamada só após a finalização do calculo
        }
        else if (IsZona)
        {
            AjustaZonaPassLevel();
        }
        else
        {
            Debug.Log("O jogo não está sendo balanceado");
        }
    }

    //ajusta o nível quando morre (GameController)
    public void BalanceAtDeath()
    {

        CalculaZona();

        float ajuste_zona = 0f;
        if (zona == PlayerState.LOW)
        {
            ajuste_zona = 0f;
        }
        else if (zona == PlayerState.NORMAL)
        {
            ajuste_zona = -0.25f;
        }
        else
        { //(zona == PlayerState.HIGH) 
            ajuste_zona = -0.5f;
        }

        if (IsAfetivo)
        {
            float ajuste_ext = 0;
            if (excitacao == PlayerState.HIGH)
            {
                ajuste_ext = -0.5f;
            }
            else if (excitacao == PlayerState.NORMAL)
            {
                ajuste_ext = -0.25f;
            }
            else if (excitacao == PlayerState.LOW)
            {
                ajuste_ext = 0f;
            }
            else
            {
                Debug.Log("Warning: É AFT e PlayerState.excitacao==NULL (BalanceAtDeath)");
                ajuste_ext = -0.25f;
            }
            asteroidSpeed += (ajuste_ext + ajuste_zona);
        }
        else if (IsZona)
        {
            asteroidSpeed += (ajuste_zona - 0.25f);// -0.25f para considerar que é desempenho ou excitacao normal
        }
        else
        {
            Debug.Log("O jogo não está sendo balanceado...");
        }
    }

    private void AjustaZonaPassLevel()
    {
        NGUIDebug.Clear();
        float ajuste_zona = 0f;
        if (zona == PlayerState.LOW)
        {
            Debug.Log("zonaL");
            NGUIDebug.Log("zl");
            ajuste_zona = 1.5f;
        }
        else if (zona == PlayerState.NORMAL)
        {
            Debug.Log("zonaN");
            NGUIDebug.Log("zn");
            ajuste_zona = 1f;
        }
        else
        { //(zona == PlayerState.HIGH) 
            Debug.Log("zonaH");
            NGUIDebug.Log("zh");
            ajuste_zona = 0.5f;
        }

        NGUIDebug.Log((ajuste_zona) + "35470" + DataCenter.instance.velMinInicial + "0" + asteroidSpeed);
        asteroidSpeed += (ajuste_zona);
    }

    public void AjustaExcitacaoPassLevel()
    {

        NGUIDebug.Clear();

        float ajuste_ext = 0f;
        if (excitacao == PlayerState.HIGH)
        {
            Debug.Log("extH");
            NGUIDebug.Log("eh");
            ajuste_ext = 0f;
        }
        else if (excitacao == PlayerState.NORMAL)
        {
            Debug.Log("extN");
            NGUIDebug.Log("en");
            ajuste_ext = 0.5f;
        }
        else if (excitacao == PlayerState.LOW)
        {
            Debug.Log("extL");
            NGUIDebug.Log("el");
            ajuste_ext = 1f;
        }
        else
        { //excitacao == NULL
            Debug.Log("Warning: excitacao == Null (AjustaExcitacao)");
            NGUIDebug.Log("e-");
            ajuste_ext = 0.5f;
        }

        float ajuste_zona = 0f;
        if (zona == PlayerState.LOW)
        {
            Debug.Log("zonaL");
            NGUIDebug.Log("zl");
            ajuste_zona = 1f;
        }
        else if (zona == PlayerState.NORMAL)
        {
            Debug.Log("zonaN");
            NGUIDebug.Log("zn");
            ajuste_zona = 0.5f;
        }
        else
        { //(zona == PlayerState.HIGH) 
            Debug.Log("zonaH");
            NGUIDebug.Log("zh");
            ajuste_zona = 0f;
        }

        Debug.Log("Ajuste pass nivel: " + (ajuste_ext + ajuste_zona) + " vel inicial: " + DataCenter.instance.velMinInicial + " vel final: " + asteroidSpeed);
        NGUIDebug.Log((ajuste_ext + ajuste_zona) + "35470" + DataCenter.instance.velMinInicial + "0" + asteroidSpeed);

        asteroidSpeed += (ajuste_ext + ajuste_zona);

    }

    public void CalculaZona()
    {
        int mortes = DataCenter.instance.numberOfLevelDeaths;
        double duracao = DataCenter.instance.GetDuracao();

        if (mortes < 4 && duracao < 67)
        {
            zona = PlayerState.LOW;
        }
        else if (mortes > 4 && duracao > 67)
        {
            zona = PlayerState.HIGH;
        }
        else
        {
            zona = PlayerState.NORMAL;
        }
    }

    public float getAsteroidSpeed()
    {
        return asteroidSpeed;
    }

}
