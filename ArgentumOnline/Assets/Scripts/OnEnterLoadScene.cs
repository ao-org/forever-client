using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class WarpingDestination
{
    public static float teleport_x=0;
    public static float teleport_y=0;

}


public class OnEnterLoadScene : MonoBehaviour
{
    public string scene = "35";
    public string teleport_x = "12";
    public string teleport_y = "7";

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("OnEnterLoadScene " + col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        if(col.CompareTag("Player")){
            SceneManager.LoadScene(scene);
            GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
            Debug.Assert(player!=null);
            float x = float.Parse(teleport_x);
            float y = float.Parse(teleport_y);
        
            Debug.Log("Teleporting player to x:" + teleport_x + " y:" + teleport_y);
            WarpingDestination.teleport_x = x;
            WarpingDestination.teleport_y = y;
        }
    }
}
