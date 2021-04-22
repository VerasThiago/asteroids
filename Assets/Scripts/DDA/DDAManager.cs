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
    public float speedChanged = 0; // Máximo que pode ser alterado por nível

    public PlayerState excitacao; //LOW, NORMAL, HIGH, NULL(quando só desempenho)
    public PlayerState zona; //LOW(amena), NORMAL(otima) ou HIGH(intensa)

    public PlayerState arousal; // Player arousal based on EDA interpretation

    private DDAAfetivo afectiveDDA;
    private DDADSA dsaDDA;
    private DDAGSR gsrDDA;

    public List<EDASignal> changes;

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
            IsAfetivo = false;
        }
        else if (typeText == "Tempo Real DSA")
        {
            Debug.Log("Selecionado tipo de ADD algoritmo DSA");
            type = ADDTypes.DSA;
            IsAfetivo = false;
        }
    }

    public void edaRequestAdjusment()
    {
        EDASignal lastElement = EDADatabase.instance.signals.eda[EDADatabase.instance.signals.eda.Count - 1];
        if (type == ADDTypes.Afective)
        {
            afectiveDDA.MedianExcitment(EDADatabase.instance.signals);
        } else if(type == ADDTypes.GSR) {

            double gsrChange = gsrDDA.GetChanges();
            changes.Add(new EDASignal(lastElement.time, gsrChange, lastElement.stringtime));
            realTimeSpeedAdjust(gsrDDA.GetChanged(gsrChange));
        } else if (type == ADDTypes.DSA)
        {
            int dsaChange = dsaDDA.AddPoints(EDADatabase.instance.signals);
            changes.Add(new EDASignal(lastElement.time, dsaChange, lastElement.stringtime));
            realTimeSpeedAdjust(dsaChange);
        }
    }

    public void realTimeSpeedAdjust(int slope)
    {
        float adjust = 0.0f;
        if (slope == 1 && DDAManager.instance.zona == PlayerState.HIGH)
        {
            adjust = -0.25f;
        }
        else if (slope == -1 && DDAManager.instance.zona == PlayerState.LOW)
        {
            adjust = 0.25f;
        }

        DDAManager.instance.speedChanged += adjust;
        if (Mathf.Abs(DDAManager.instance.speedChanged) <= 1 && adjust != 0.0f) {
            DDAManager.instance.asteroidSpeed += adjust;
            DataCenter.instance.AddSpeedAdjustment(adjust);
        }
    }

    public void passLevelAdjustment()
    {
        if(type == ADDTypes.Afective)
        {
            afectiveDDA.BalanceAtPassLevel();
        }
        else
        {
            DDAManager.instance.asteroidSpeed += 1.0f;
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
