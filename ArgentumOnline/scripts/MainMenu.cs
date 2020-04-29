/*
    Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
    Argentum Online Clasico
    noland.studios@gmail.com
*/
using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(){
      InputField server_address_input = GameObject.Find("ServerIPInputField").GetComponent<InputField>();
      InputField server_port_input    = GameObject.Find("ServerPortInputField").GetComponent<InputField>();
      string server_address_string    = server_address_input.text;
      string server_port_string       = server_port_input.text;
      try {
        Debug.Log("Server address: " + server_address_string + ":" + server_port_string);
        GameObject tcp_client_object = GameObject.FindGameObjectsWithTag("TCPClient")[0];

        //Attempt to connect to game Server
        TCPClient client = tcp_client_object.GetComponent<TCPClient>();
        client.ConnectToTcpServer(server_address_string,server_port_string);
        SceneManager.LoadScene("World");
      }
      catch (Exception e) {
			     Debug.Log("Failed to connect to server " + e);
		  }

    }

    public void QuitGame(){
        Debug.Log("QuitGame");
        Application.Quit();
    }
}
