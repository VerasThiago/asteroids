using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Malha : MonoBehaviour {
    public GameObject c_vert;
    public GameObject c_horiz;

    private int tamanho_malha = 4;

    void Start() {
        var c_vert_x = -200;
        while (c_vert_x < 500) {
            GameObject cylinder_vert = (GameObject)Instantiate(c_vert, new Vector3(c_vert_x, 0, -4), Quaternion.identity);
            c_vert_x = c_vert_x + tamanho_malha;
        }

        var c_horiz_x = -300;
        while (c_horiz_x < 270) {
            GameObject cylinder_horiz = (GameObject)Instantiate(c_horiz, new Vector3(0, c_horiz_x, -4), Quaternion.Euler(0,0,90));
            c_horiz_x = c_horiz_x + tamanho_malha;
        }

        c_vert.SetActive(false);
        c_horiz.SetActive(false);
    }

}