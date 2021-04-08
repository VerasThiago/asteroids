using UnityEngine;
using System.Collections;

public class DDADSA
{
    private int prev;
    private int curr;

    private int sampleCount;

    private int totalChange;
    private int currentSampleCount;
    private int threshold;

    // Use this for initialization
    public DDADSA()
    {
        prev = -1;
        curr = -1;
        currentSampleCount = 0;
        totalChange = 0;
        sampleCount = 20;
        threshold = 20;
    }

    public int AddDataPoint(int newData)
    {
        prev = curr;
        curr = newData;

        if (prev != -1 && curr != -1)
        {
            totalChange += curr - prev;
            currentSampleCount++;

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
