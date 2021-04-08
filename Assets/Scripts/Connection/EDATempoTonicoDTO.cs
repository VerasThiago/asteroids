using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EDATempoTonicoDTO 
{
    public List<float> tempoEda; //eixo x do grafico edaxtempo
    public List<double> eda; //eixo y do grafico edaxtempo
    public List<float> tempoTonicLevel; //eixo x do grafico tonicoxtempo e phasicxtempo
    public List<int> tonicLevel; //eixo y do grafico tonicoxtempo
    public List<int> phasicLevel; //eixo y do grafico phasicxtempo
}
