using UnityEngine;
using System.Collections;

public class DDAGSR 
{
    private EDAProcessor edaProcessor;

    private double lastArousal = 0;

    // Use this for initialization
    public DDAGSR()
    {
        edaProcessor = new EDAProcessor();
    }

    public double GetChanges()
    {
        int tonic = edaProcessor.GetTonicLevel(EDADatabase.instance.signals.eda);
        int phasic = edaProcessor.GetPhasicLevel(EDADatabase.instance.signals.eda); // quanto maior a diferença entre picos por baixo, maior será o valor. Quanto maior por cima, meno
        float change = (((float)(phasic - 50.0f)) / 100000.0f) - (((float)(tonic - 50.0f)) / 100000.0f);
        float arousal = edaProcessor.GetGeneralArousalLevel(EDADatabase.instance.signals.eda);


        Debug.Log("Tônico: " + tonic + " Fásico: " + phasic + " Alteração:" + change + " Excitação:" + arousal);

        return arousal;
    }

    public int GetChanged(double arousal)
    {
        if(lastArousal != 0)
        {
            if(arousal > 1.3 * lastArousal)
            {
                lastArousal = arousal;
                return 1;
            }
            else if (arousal < 0.7 * lastArousal)
            {
                lastArousal = arousal;
                return -1;
            }
        }
        lastArousal = arousal;
   
        return 0;
    } 
}
