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
using UnityEditor;

public class MainMenu : MonoBehaviour
{
    public void ShowMessageBox(string title,string text)
    {
        Text TitleText = GameObject.Find("MsgBoxTitle").GetComponent<Text>();
        Debug.Assert(TitleText!=null);
        TitleText.text = title;
        Text BodyText = GameObject.Find("MsgBoxText").GetComponent<Text>();
        Debug.Assert(BodyText!=null);
        BodyText.text = text;
        GameObject mm = GameObject.Find("MessageBox");
		Debug.Assert(mm!=null);
        mm.gameObject.SetActive (true);
    }
    public void PlayGame(){
      InputField server_address_input = GameObject.Find("ServerIPInputField").GetComponent<InputField>();
      InputField server_port_input    = GameObject.Find("ServerPortInputField").GetComponent<InputField>();
      InputField username_input       = GameObject.Find("UsernameInputField").GetComponent<InputField>();
      InputField password_input       = GameObject.Find("PasswordInputField").GetComponent<InputField>();

      string username_str             = username_input.text;
      string password_str             = password_input.text;
      string server_address_string    = server_address_input.text;
      string server_port_string       = server_port_input.text;

      if(username_str == null || username_str.Length<3){
        //  EditorUtility.DisplayDialog("Username invalido","Por favor ingrese un username valido", "OK");
          return;
      }
      if(password_str == null || password_str.Length<3){
          //EditorUtility.DisplayDialog("Password invalido","Por favor ingrese un password valido", "OK");
          return;
      }

      try {
        //Attempt to connect to game Server
        GameObject tcp_client_object = GameObject.FindGameObjectsWithTag("TCPClient")[0];
        TCPClient client = tcp_client_object.GetComponent<TCPClient>();
        if( client.IsConnected()){
            client.AttemptToLogin();
        }
        else {
            Debug.Log("Server address: " + server_address_string + ":" + server_port_string);
            client.SetMainMenu(this);
            client.SetUsernameAndPassword(username_str,password_str);
            client.ConnectToTcpServer(server_address_string,server_port_string);
        }
        //SceneManager.LoadScene("World");
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
