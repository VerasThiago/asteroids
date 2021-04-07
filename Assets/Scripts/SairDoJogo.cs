using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SairDoJogo : MonoBehaviour {

    public GameObject cameraIcon;

    private void Start() {

        cameraIcon.SetActive(false);

    }
    private void Update() {
        if (Input.GetKey(KeyCode.Q)) {
            if (Input.GetKey(KeyCode.W)) {
                cameraIcon.SetActive(true);
            }
            else {
                cameraIcon.SetActive(false);
            }
        }
    }

    public void Sair() {
        Application.Quit();
    }

    void OnApplicationQuit() {
        if (SceneManager.GetActiveScene().name.Equals("Fim do Jogo")) {
            DataCenter.instance.Write();
        }
    }
}
