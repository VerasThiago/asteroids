using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// JSON que vai ser gerado, possui [System.Serializable] e não implementa MonoBehaviour justamente por causa disso
// contém as informações de cada level
[System.Serializable]
public class DataFileSignals
{
    public List<EDASignal> signals;
    public List<EDASignal> arousals;
}
