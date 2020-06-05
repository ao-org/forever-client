using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera1 : MonoBehaviour
{
    //private Camera mCamera;
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.orthographicSize = 9;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKey(KeyCode.Q)) // Change From Q to anyother key you want
        {
            //camera.orthographicSize = camera.orthographicSize + 1 * Time.deltaTime;
            Camera.main.orthographicSize = Camera.main.orthographicSize - 1 * Time.deltaTime; ;
            if (Camera.main.orthographicSize < 4)
            {
                Camera.main.orthographicSize = 4; // Max size
            }
        }
        if (Input.GetKey(KeyCode.E)) // Change From Q to anyother key you want
        {
            Camera.main.orthographicSize = Camera.main.orthographicSize + 1 * Time.deltaTime;
            if (Camera.main.orthographicSize > 12)
            {
                Camera.main.orthographicSize = 12; // Max size
            }
        }*/
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            float delta = 1.0f; //Cambiar esta variable para acelerar o no el scroll del mouse
            Camera.main.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * delta;
            if (Camera.main.orthographicSize > 12)
                Camera.main.orthographicSize = 12;
            if (Camera.main.orthographicSize < 4)
                Camera.main.orthographicSize = 4;
        }
    }
}
