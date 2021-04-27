using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DDADSA
{
    private double prev;
    private double curr;
    private List<double> values = new List<double>();

    private int sampleCount;

    private double totalChange;
    private int currentSampleCount;
    // private double threshold;
    private double deviation = 0;

    public DDADSA()
    {
        prev = -1;
        curr = -1;
        currentSampleCount = 0;
        totalChange = 0;
        sampleCount = 20;
        // threshold = 20;
        deviation = 0;
    }

    public int AddDataPoint(double newData)
    {
        prev = curr;
        curr = newData;

        if (prev != -1 && curr != -1)
        {
            totalChange += curr - prev;
            values.Add(curr);
            currentSampleCount++;

            
            if (currentSampleCount == sampleCount)
            {
                //Debug.Log(deviation);
                //Debug.Log("c: " + Mathf.Abs((float)totalChange) + " th: " + deviation);
                //Debug.Log(Mathf.Abs((float)totalChange) > deviation);

                if (Mathf.Abs((float)totalChange) > deviation &&  deviation != 0)
                {
                    if (totalChange > 0)
                    {
                        ResetHistogram();
                        return 1;

                    }
                    else if (totalChange < 0)
                    {
                        ResetHistogram();
                        return -1;
                    }
                }
                ResetHistogram();
            }
        }
        return 0;
    }

    // Update is called once per frame
    void ResetHistogram()
    {
        saveDeviation();
        currentSampleCount = 0;
        totalChange = 0;
        values.Clear();
    }

    void saveDeviation()
    {
        double avg = values.Average();
        deviation = Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
    }

    public int AddPoints (EDASignals signals)
    {
        int change = 0;
        if (signals.eda.Count > 0)
        {
            for (int i = 0; i < signals.eda.Count; i++)
            {
                if (change == 0)
                {
                    change = AddDataPoint(signals.eda[i].value);
                }  
            }
            return change;
        }

        return 0;
    } 
}
