using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class WarpingDestination
{
    public static float teleport_x=0;
    public static float teleport_y=0;
    public static bool warping = false;
    public static Movement.Direction direction= Movement.Direction.North;

}


public class OnEnterLoadScene : MonoBehaviour
{
    public string scene = "35";
    public string teleport_x = "12";
    public string teleport_y = "7";
    public bool locky = false;
    public bool lockx = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        UnityEngine.Debug.Log("OnEnterLoadScene " + col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        if(col.CompareTag("Player")){
            SceneManager.LoadScene(scene);
            GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
            UnityEngine.Debug.Assert(player!=null);

            if(!lockx){
                float x = float.Parse(teleport_x);
                WarpingDestination.teleport_x = x;
            }
            else {
                WarpingDestination.teleport_x = player.transform.position.x;
            }
            if (!locky){
                float y = float.Parse(teleport_y);
                WarpingDestination.teleport_y = y;
            }
            else {
                WarpingDestination.teleport_y = player.transform.position.y;
            }
            UnityEngine.Debug.Log("Teleporting player to x:" + WarpingDestination.teleport_x + " y:" + WarpingDestination.teleport_y);
            WarpingDestination.direction=player.GetComponent<Movement>().GetDirection();
            WarpingDestination.warping = true;
        }
        else {
            UnityEngine.Debug.Log("Error! No puedo encontrar el tag PLAYER!");
        }
    }
}
