using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using State;

// JSON que vai ser gerado, possui [System.Serializable] e não implementa MonoBehaviour justamente por causa disso
// level (DataFileLevel) contem as informações de cada nível.
[System.Serializable]
public class DataFileEDA {
    
    public DataFileSignals edaSignals; //não imprime no json por ser private

    public DataFileEDA(){
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US"); //para imprimir separar os floats com '.' e nao ','
        edaSignals = new DataFileSignals();
    }

    public void addSignalsOnFile(List<EDASignal> eda, List<EDASignal> arousals) {
        edaSignals.signals = eda;
        edaSignals.arousals = arousals;
    }
}
