using UnityEngine;
using System.Collections;

public class DDADSA
{
    private double prev;
    private double curr;

    private int sampleCount;

    private double totalChange;
    private int currentSampleCount;
    private double threshold;

    public DDADSA()
    {
        prev = -1;
        curr = -1;
        currentSampleCount = 0;
        totalChange = 0;
        sampleCount = 20;
        threshold = 20;
    }

    public int AddDataPoint(double newData)
    {
        prev = curr;
        curr = newData;

        if (prev != -1 && curr != -1)
        {
            totalChange += curr - prev;
            currentSampleCount++;

            
            if (currentSampleCount == sampleCount)
            {
                //Debug.Log("c: " + Mathf.Abs((float)totalChange) + " th: " + (curr * (threshold / 100)));
                //Debug.Log(Mathf.Abs((float)totalChange) > (curr * (threshold / 100)));

                if (Mathf.Abs((float)totalChange) > (curr * (threshold / 100)))
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
        currentSampleCount = 0;
        totalChange = 0;

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
