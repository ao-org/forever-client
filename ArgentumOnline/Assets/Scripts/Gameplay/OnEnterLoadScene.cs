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
        if (col.CompareTag("Player"))
        {
            
            GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
            UnityEngine.Debug.Assert(player != null);
            PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
            playerScript.Start();
            UnityEngine.Debug.Assert(player != null);

            /*if (!lockx){
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
            camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -1);
            UnityEngine.Debug.Log("Teleporting player to x:" + WarpingDestination.teleport_x + " y:" + WarpingDestination.teleport_y);
            WarpingDestination.direction= player.GetComponent<Movement>().GetDirection();
            WarpingDestination.warping = true;
        }
        else {
            UnityEngine.Debug.Log("Error! No puedo encontrar el tag PLAYER!");
        }*/
            Vector3 newPos = new Vector3(0, 0, 0);
            if (!lockx)
            {
                float x = float.Parse(teleport_x);
                newPos.x = x;
                UnityEngine.Debug.Log("TELEPORT X: " + newPos.x.ToString());
            }
            else
            {
                newPos.x = player.transform.position.x;
                UnityEngine.Debug.Log("TELEPORT X (Locked): " + newPos.x.ToString());
            }
            if (!locky)
            {
                float y = float.Parse(teleport_y);
                newPos.y = y;
                UnityEngine.Debug.Log("TELEPORT Y: " + newPos.y.ToString());
            }
            else
            {
                newPos.y = player.transform.position.y;
                UnityEngine.Debug.Log("TELEPORT Y (Locked): " + newPos.y.ToString());
            }
            UnityEngine.Debug.Log("TELEPORT X Y: " + player.transform.position.x.ToString() + player.transform.position.y.ToString());
            UnityEngine.Debug.Log("Scene Name: " + scene);
            //SceneManager.LoadScene(scene);
            player.transform.position = new Vector3(newPos.x, newPos.y, 0);
            SceneManager.LoadScene(scene);
            UnityEngine.Debug.Log("TELEPORT X Y: " + player.transform.position.x.ToString() + player.transform.position.y.ToString());
            
            UnityEngine.Debug.Log("Teleporting player to x:" + WarpingDestination.teleport_x + " y:" + WarpingDestination.teleport_y);
            WarpingDestination.direction = player.GetComponent<PlayerMovement>().GetDirection();
            WarpingDestination.warping = true;
            
        }
    }
}
