using UnityEngine;
using System.Collections;

public class DDAGSR 
{
    private EDAProcessor edaProcessor;

    // Use this for initialization
    public DDAGSR()
    {
        edaProcessor = new EDAProcessor();
    }

    public void getChanges()
    {
        int tonic = edaProcessor.GetTonicLevel(EDADatabase.instance.signals.eda);
        int phasic = edaProcessor.GetPhasicLevel(EDADatabase.instance.signals.eda); // quanto maior a diferença entre picos por baixo, maior será o valor. Quanto maior por cima, meno
        float change = (((float)(phasic - 50.0f)) / 100000.0f) - (((float)(tonic - 50.0f)) / 100000.0f);

        Debug.Log("Tônico: " + tonic + " Fásico: " + phasic + " Alteração:" + change);
    }
}
