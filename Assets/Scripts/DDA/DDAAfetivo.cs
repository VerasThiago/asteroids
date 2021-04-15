using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using State;

public class DDAAfetivo
{
    List<PicoEDA> picos = new List<PicoEDA>();

    //chamada quando se passa de nível (PassLevel)
    public void BalanceAtPassLevel()
    {
        CalculaZona();

        EDADatabase.instance.GetEDAFromDB(true, false);
        //le os sinais e os salva em EDADatabase.instance.sinais
        //O  ajuste de excitacao só é chamado dps de calcular o desempenho,
        // e como ele é concorrente, ela é chamada só após a finalização do calculo

    }

    //ajusta o nível quando morre (GameController)
    public void BalanceAtDeath()
    {

        CalculaZona();

        float ajuste_zona = 0f;
        if (DDAManager.instance.zona == PlayerState.LOW)
        {
            ajuste_zona = 0f;
        }
        else if (DDAManager.instance.zona == PlayerState.NORMAL)
        {
            ajuste_zona = -0.25f;
        }
        else
        { //(zona == PlayerState.HIGH) 
            ajuste_zona = -0.5f;
        }

       
        float ajuste_ext = 0;
        if (DDAManager.instance.excitacao == PlayerState.HIGH)
        {
            ajuste_ext = -0.5f;
        }
        else if (DDAManager.instance.excitacao == PlayerState.NORMAL)
        {
            ajuste_ext = -0.25f;
        }
        else if (DDAManager.instance.excitacao == PlayerState.LOW)
        {
            ajuste_ext = 0f;
        }
        else
        {
            Debug.Log("Warning: É AFT e PlayerState.excitacao==NULL (BalanceAtDeath)");
            ajuste_ext = -0.25f;
        }
        DDAManager.instance.asteroidSpeed += (ajuste_ext + ajuste_zona);
      
    }

    public void AjustaExcitacaoPassLevel()
    {

        NGUIDebug.Clear();

        float ajuste_ext = 0f;
        if (DDAManager.instance.excitacao == PlayerState.HIGH)
        {
            Debug.Log("extH");
            NGUIDebug.Log("eh");
            ajuste_ext = 0f;
        }
        else if (DDAManager.instance.excitacao == PlayerState.NORMAL)
        {
            Debug.Log("extN");
            NGUIDebug.Log("en");
            ajuste_ext = 0.5f;
        }
        else if (DDAManager.instance.excitacao == PlayerState.LOW)
        {
            Debug.Log("extL");
            NGUIDebug.Log("el");
            ajuste_ext = 1f;
        }
        else
        { //excitacao == NULL
            Debug.Log("Warning: excitacao == Null (AjustaExcitacao)");
            NGUIDebug.Log("e-");
            ajuste_ext = 0.5f;
        }

        float ajuste_zona = 0f;
        if (DDAManager.instance.zona == PlayerState.LOW)
        {
            Debug.Log("zonaL");
            NGUIDebug.Log("zl");
            ajuste_zona = 1f;
        }
        else if (DDAManager.instance.zona == PlayerState.NORMAL)
        {
            Debug.Log("zonaN");
            NGUIDebug.Log("zn");
            ajuste_zona = 0.5f;
        }
        else
        { //(zona == PlayerState.HIGH) 
            Debug.Log("zonaH");
            NGUIDebug.Log("zh");
            ajuste_zona = 0f;
        }

        Debug.Log("Ajuste pass nivel: " + (ajuste_ext + ajuste_zona) + " vel inicial: " + DataCenter.instance.velMinInicial + " vel final: " + DDAManager.instance.asteroidSpeed);
        NGUIDebug.Log((ajuste_ext + ajuste_zona) + "35470" + DataCenter.instance.velMinInicial + "0" + DDAManager.instance.asteroidSpeed);

        DDAManager.instance.asteroidSpeed += (ajuste_ext + ajuste_zona);
    }

    public void CalculaZona()
    {
        int mortes = DataCenter.instance.numberOfLevelDeaths;
        double duracao = DataCenter.instance.GetDuracao();

        if (mortes < 4 && duracao < 67)
        {
            DDAManager.instance.zona = PlayerState.LOW;
        }
        else if (mortes > 4 && duracao > 67)
        {
            DDAManager.instance.zona = PlayerState.HIGH;
        }
        else
        {
            DDAManager.instance.zona = PlayerState.NORMAL;
        }
    }

    private void CalculaPicos(EDASignals sinais)
    {
        bool estava_subindo = false;
        bool estava_descendo = false;
        double tamanho = 0;
        double dif;
        int ignorar = 10; //numero de valores a se ignorar. o valor inicial pode ter mais relação com o questionario do que com o jogo em si. 6 foi encontrado por testes como um bom número

        sinais.eda.Sort((x, y) => x.id.CompareTo(y.id)); //se nao tiver em ordem, ordena os sinais por id

        //se tiver menos sinais do que ignorar+2, nao calcula a excitacao
        if (sinais.eda.Count < ignorar + 2)
        {
            picos.Clear();
        }
        else
        {
            for (int i = 0; i < sinais.eda.Count; i++)
            {
                //descarta os 20 primeiros sinais
                if (i == ignorar)
                {
                    dif = sinais.eda[ignorar].value - sinais.eda[ignorar - 1].value;
                    if (dif >= 0)
                    {
                        estava_subindo = true;
                        tamanho = dif;
                    }
                    else
                    {
                        estava_descendo = true;
                        tamanho = -dif;
                    }
                }
                else if (i > ignorar)
                {
                    dif = sinais.eda[i].value - sinais.eda[i - 1].value;
                    //se subindo agora
                    if (dif > 0)
                    {
                        //se já estava subindo
                        if (estava_subindo)
                        {
                            tamanho += dif;
                        }
                        //começou a subir só agora. encontrou um pico negativo em i-1
                        else if (estava_descendo)
                        {
                            //add pico
                            picos.Add(new PicoEDA(sinais.eda[i - 1], tamanho));
                            tamanho = dif;
                            estava_descendo = false;
                            estava_subindo = true;
                        }
                        else
                        {
                            Debug.Log("Um erro está acontecendo ao calcular o EDA. (nao está detectando subida nem descida) (1)");
                        }
                    }
                    //se descendo agora
                    else
                    {
                        //se já estava descendo
                        if (estava_descendo)
                        {
                            tamanho -= dif;
                        }
                        //comçou a descer só agora. encontrou um pico positivo em i-1
                        else if (estava_subindo)
                        {
                            //add pico;
                            picos.Add(new PicoEDA(sinais.eda[i - 1], tamanho));
                            tamanho = -dif;
                            estava_subindo = false;
                            estava_descendo = true;
                        }
                        else
                        {
                            Debug.Log("Um erro está acontecendo ao calcular o EDA. (nao está detectando subida nem descida) (2)");
                        }
                    }
                }
            }
        }
    }

    private void CaclulaExcitacao()
    {
        double edaInicialMedia;
        double edaFinalMedia;
        int s = picos.Count;
        if (s > 3)
        {
            edaInicialMedia = (picos[0].value + picos[1].value + picos[2].value + picos[3].value) / 4;
            edaFinalMedia = (picos[s - 1].value + picos[s - 2].value + picos[s - 3].value + picos[s - 4].value) / 4;
        }
        else
        {
            edaInicialMedia = picos[0].value;
            edaFinalMedia = picos[s - 1].value;
        }
        picos.Sort((x, y) => x.size.CompareTo(y.size)); 
        double ruido = picos[(int)(picos.Count / 2)].size; 

        int escala = 2; 

        Debug.Log("edaInicialMedia: " + edaInicialMedia);
        Debug.Log("edaFinalMedia: " + edaFinalMedia);
        Debug.Log("ruido: " + ruido);
        //subiu
        if (edaFinalMedia - edaInicialMedia > escala * ruido)
        {
            Debug.Log("Excitação: HIGH");
            DDAManager.instance.excitacao = State.PlayerState.HIGH;
        }
        //desceu
        else if (edaFinalMedia - edaInicialMedia < -1 * escala * ruido)
        {
            Debug.Log("Excitação: LOW");
            DDAManager.instance.excitacao = State.PlayerState.LOW;
        }
        else
        {
            Debug.Log("Excitação: NORMAL");
            DDAManager.instance.excitacao = State.PlayerState.NORMAL;
        }
    }

    public void MedianExcitment(EDASignals sinais)
    {
        picos.Clear();
        CalculaPicos(sinais); //pontos máximos e minimos relativos
        NGUIDebug.Log(picos.Count + "pic");
        Debug.Log(picos.Count + " picos achados");
        if (picos.Count > 1)
        {
            CaclulaExcitacao();
        }
        else if (sinais.eda.Count > 0)
        {
            Debug.Log("Warning: Excitação: NORMAL (picos.Count <= 0");
            DDAManager.instance.excitacao = State.PlayerState.NORMAL; //normal pq temos poucos sinais, entao pouca variacao
        }
        else
        {
            Debug.Log("Warning: Excitação: NULL (sinais.eda.Count <= 0");
            DDAManager.instance.excitacao = State.PlayerState.NULL; //null pq nao temos nenhum sinal
        }

        AjustaExcitacaoPassLevel();
    }
}
