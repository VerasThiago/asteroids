using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Presentation : MonoBehaviour
{
    public string nextScene;
    private bool jaClicouSubmit = false;
    public GameObject nome_go;
    public GameObject sobrenome_go;
    EventSystem system;

    // Start is called before the first frame update
    void Start()
    {
        system = EventSystem.current;
        system.SetSelectedGameObject(nome_go, new BaseEventData(system)); //começa o jogo com o pointer no nome
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            system.SetSelectedGameObject(sobrenome_go, new BaseEventData(system)); //pointer vai pro sobrenome
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (Input.GetKeyDown(KeyCode.Tab)) {
                system.SetSelectedGameObject(nome_go, new BaseEventData(system)); //pointer volta pro nome
            }
        }
        if (Input.GetKeyDown(KeyCode.Return)) {
            //BotaoJogar(); por enquanto não implementar isso pq não clicar no botão jogar, não chama o SensorChooser.SaveSensor
        }

        // para deixar as caixas vermelhas se clicar submit mas não estiver escrito nada 
        if (jaClicouSubmit) {
            string nome_str = nome_go.transform.Find("Texto").GetComponent<Text>().text;
            if (nome_str.Equals("")) {
                nome_go.GetComponent<Image>().color = new UnityEngine.Color32(246,115,115,255);
            }
            else {
                nome_go.GetComponent<Image>().color = new UnityEngine.Color32(255,255,255,255);
            }

            string sobrenome_str = sobrenome_go.transform.Find("Texto").GetComponent<Text>().text;
            if (sobrenome_str.Equals("")) {
                sobrenome_go.GetComponent<Image>().color = new UnityEngine.Color32(246, 115, 115, 255);
            }
            else {
                sobrenome_go.GetComponent<Image>().color = new UnityEngine.Color32(255, 255, 255, 255);
            }
        }
        
    }

    public void BotaoJogar() {
        jaClicouSubmit = true;

        string nome_str = nome_go.transform.Find("Texto").GetComponent<Text>().text;
        string sobrenome_str = sobrenome_go.transform.Find("Texto").GetComponent<Text>().text;

        if(!nome_str.Equals("") && !sobrenome_str.Equals("")) {
            DataCenter.instance.SetNomeJogador(nome_str, sobrenome_str);
            SceneManager.LoadScene(nextScene);
            Destroy(gameObject);
        }
    }
}
