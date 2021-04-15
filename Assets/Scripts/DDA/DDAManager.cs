using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using State;

public class DDAManager : MonoBehaviour
{

    public GameObject prefab;
    public static DDAManager instance;

    public float asteroidSpeed = 1f; //velocidade dos asteroids

    public PlayerState excitacao; //LOW, NORMAL, HIGH, NULL(quando só desempenho)
    public PlayerState zona; //LOW(amena), NORMAL(otima) ou HIGH(intensa)

    public PlayerState arousal; // Player arousal based on EDA interpretation

    private DDAAfetivo afectiveDDA;
    private DDADSA dsaDDA;
    private DDAGSR gsrDDA;

    //private float EDA = 0; //eda values

    public enum ADDTypes { None, Afective, GSR, DSA };
    public ADDTypes type = ADDTypes.None;

    public bool IsAfetivo = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = prefab.GetComponent<DDAManager>();
            excitacao = PlayerState.NULL;
            zona = PlayerState.LOW; // Starting player zone with low deaths

            afectiveDDA = new DDAAfetivo();
            dsaDDA = new DDADSA();
            gsrDDA = new DDAGSR();
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
            IsAfetivo = false;
        }
        else if (typeText == "Afetivo")
        {
            Debug.Log("Selecionado tipo de ADD afetivo (legado)");
            type = ADDTypes.Afective;
            IsAfetivo = true;
        }
        else if (typeText == "Tempo Real GSR")
        {
            Debug.Log("Selecionado tipo de ADD algoritmo GSR");
            type = ADDTypes.GSR;
            IsAfetivo = true;
        }
        else if (typeText == "Tempo Real DSA")
        {
            Debug.Log("Selecionado tipo de ADD algoritmo DSA");
            type = ADDTypes.DSA;
            IsAfetivo = true;
        }
    }

    public void edaRequestAdjusment()
    {
        if (type == ADDTypes.Afective)
        {
            afectiveDDA.MedianExcitment(EDADatabase.instance.signals);
        }
    }

    public void passLevelAdjustment()
    {
        if(type == ADDTypes.Afective)
        {
            afectiveDDA.BalanceAtPassLevel();
        }
    }

    public void deathAdjustment()
    {
        if (type == ADDTypes.Afective)
        {
            afectiveDDA.BalanceAtDeath();
        }
    }

    public float getAsteroidSpeed()
    {
        return asteroidSpeed;
    }

}
