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

            Debug.Log(totalChange);
            if (currentSampleCount == sampleCount)
            {
                if (totalChange >= threshold)
                {
                    ResetHistogram();
                    return 1;

                }
                else if (totalChange < 0 && totalChange <= threshold*-1)
                {
                    ResetHistogram();
                    return -1;
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
}
