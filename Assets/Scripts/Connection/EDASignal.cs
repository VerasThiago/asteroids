using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Usado pelo EDASignals, representa a leitura do EDA em um instante
[System.Serializable]
public class EDASignal
{
    public int id;
    public double value;
    public double time;
    public int read; //não estamos usando. estava no codigo antigo

    public EDASignal(int id, double time, double value, int read) {
        this.id = id;
        this.time = time;
        this.value = value;
        this.read = read;
    }

    public EDASignal(double time, double value) {
        this.time = time;
        this.value = value;
    }
}
