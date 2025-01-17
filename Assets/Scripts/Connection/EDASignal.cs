﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Usado pelo EDASignals, representa a leitura do EDA em um instante
[System.Serializable]
public class EDASignal
{
    public int id;
    public double value;
    public double time;
    public string stringtime;
   // public int read;

    public EDASignal(int id, double time, double value, string stringtime) {
        this.id = id;
        this.time = time;
        this.value = value;
        this.stringtime = stringtime;
        // this.read = read;
    }

    public EDASignal(double time, double value, string stringtime) {
        this.time = time;
        this.value = value;
        this.stringtime = stringtime;
    }

    public EDASignal(double time, double value)
    {
        this.time = time;
        this.value = value;
    }
}
