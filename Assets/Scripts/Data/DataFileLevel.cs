using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// JSON que vai ser gerado, possui [System.Serializable] e não implementa MonoBehaviour justamente por causa disso
// contém as informações de cada level
[System.Serializable]
public class DataFileLevel
{
    public int numeroDeAsteroids;
    public float velocidadeMinimaDosAsteroidsInicial; //vel min dos asteroides ao iniciar o nivel
    public float velocidadeMinimaDosAsteroids; //vel min dos asteroides ao terminar o nivel
    public float velocidadeMaximaDosAsteroids; //vel max dos asteroides ao terminar o nivel
    public double tempoInicial = 0; //em ticks
    public double tempoFinal; //em ticks
    public double tempoDuracao; //em segundos
    public float tempoPorVida;

    // public string desempenho; //esse é o desempenho desde nivel que será usado para ajudar a dificuldade do próximo nível
    public string excitacao; //esse é a excitação desde nivel que será usado para ajudar a dificuldade do próximo nível
    public string zona;

    public List<double> ajusteRealizado = new List<double>();
    public List<float> valorAjusteRealizado = new List<float>();

    public bool venceu;

    public int totalDeMortes;
    public List<double> mortes = new List<double>();

    /*public int totalDeTiros;
    public float percentualUp;
    public float percentualDown;
    public float percentualLeft;
    public float percentualRight;
    public List<double> tiros = new List<double>();
    public List<double> apertouUp = new List<double>();
    public List<double> soltouUp = new List<double>();
    public List<double> apertouDown = new List<double>();
    public List<double> soltouDown = new List<double>();
    public List<double> apertouLeft = new List<double>();
    public List<double> soltouLeft = new List<double>();
    public List<double> apertouRight = new List<double>();
    public List<double> soltouRight = new List<double>();*/

    public string dificuldade;
    public string tedio;
    public string frustracao;
    public string diversao;
    public string opiniao;
}
