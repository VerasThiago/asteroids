using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Usado para serializar um JSON com as informações do EDA vindas da camada REST
[System.Serializable]
public class EDASignals
{
    public int success;
    public List<EDASignal> eda;
}
