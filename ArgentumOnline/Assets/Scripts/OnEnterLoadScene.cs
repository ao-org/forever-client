using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class OnEnterLoadScene : MonoBehaviour
{
    public string scene = "35";

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("OnEnterLoadScene " + col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        SceneManager.LoadScene(scene);
    }
}
