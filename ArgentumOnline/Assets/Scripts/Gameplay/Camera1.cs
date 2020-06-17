using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera1 : MonoBehaviour
{
    //private Camera mCamera;
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.orthographicSize = 7;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            float delta = 1.0f; //Cambiar esta variable para acelerar o no el scroll del mouse
            Camera.main.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * delta;
            if (Camera.main.orthographicSize > 7)
                Camera.main.orthographicSize = 7;
            if (Camera.main.orthographicSize < 4)
                Camera.main.orthographicSize = 4;
        }
    }
}
