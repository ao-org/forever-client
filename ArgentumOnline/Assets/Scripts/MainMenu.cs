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
using UnityEngine.Localization;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    private EventSystem mEventSystem;

    public LocalizedString LoginErrText_MSGBOX_TITLE;
    public LocalizedString LoginErrText_USER_ALREADY_HOLDS_ACTIVE_SESSION;
    public LocalizedString LoginErrText_ACCOUNT_DOESNT_EXIST;

    private void CreateAndInitLocalizedStrings(){
        mLocalizedStringMappings = new Dictionary<string,LocalizedString>();
        mLocalizedStringMappings["LOGIN_ERROR_MSG_BOX_TITLE"]= LoginErrText_MSGBOX_TITLE;
        mLocalizedStringMappings["USER_ALREADY_HOLDS_ACTIVE_SESSION"]= LoginErrText_USER_ALREADY_HOLDS_ACTIVE_SESSION;
        mLocalizedStringMappings["ACCOUNT_DOESNT_EXIST"]= LoginErrText_ACCOUNT_DOESNT_EXIST;
    }


    public void OnApplicationQuit(){
            Debug.Log("Application ending after " + Time.time + " seconds");
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Tab)){
            Selectable next = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ?
            mEventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp() :
            mEventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                inputfield.OnPointerClick(new PointerEventData(mEventSystem));
                mEventSystem.SetSelectedGameObject(next.gameObject);
            }
            //Here is the navigating back part:
            else {
                next = Selectable.allSelectables[0];
                mEventSystem.SetSelectedGameObject(next.gameObject);
            }
        }
    }
    private TCPClient mTcpClient;
    private void Start()
    {
         CreateAndInitLocalizedStrings();
         mEventSystem = EventSystem.current;
         GameObject tcp_client_object = GameObject.FindGameObjectsWithTag("TCPClient")[0];
         mTcpClient = tcp_client_object.GetComponent<TCPClient>();
         mTcpClient.SetMainMenu(this);
    }
    void Awake(){
        //var translatedText = LocalizationSettings.StringDatabase.GetLocalizedString("PLAY_BUTTON");
        //Debug.Log("Translated Text: " + translatedText);
    }
    private IDictionary<string,LocalizedString> mLocalizedStringMappings;

    public void ShowMessageBox(string title,string text, bool localize = false)
    {
        string final_title_string = title;
        string final_text_string  = text;
        if(localize){
            Debug.Assert(mLocalizedStringMappings.ContainsKey(title));
            Debug.Assert(mLocalizedStringMappings.ContainsKey(text));
            var localizedText = mLocalizedStringMappings[title].GetLocalizedString();
            Debug.Assert(localizedText.IsDone);
            Debug.Log("LocalizedString " + localizedText.Result);
            final_title_string = localizedText.Result;
            localizedText = mLocalizedStringMappings[text].GetLocalizedString();
            Debug.Assert(localizedText.IsDone);
            Debug.Log("LocalizedString " + localizedText.Result);
            final_text_string = localizedText.Result;
        }
        Text TitleText = GameObject.Find("MsgBoxTitle").GetComponent<Text>();
        Debug.Assert(TitleText!=null);
        TitleText.text = final_title_string;
        Text BodyText = GameObject.Find("MsgBoxText").GetComponent<Text>();
        Debug.Assert(BodyText!=null);
        BodyText.text = final_text_string;
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
        //GameObject tcp_client_object = GameObject.FindGameObjectsWithTag("TCPClient")[0];
        //TCPClient client = tcp_client_object.GetComponent<TCPClient>();
        if( mTcpClient.IsConnected()){
            mTcpClient.AttemptToLogin();
        }
        else {
            Debug.Log("Server address: " + server_address_string + ":" + server_port_string);
            mTcpClient.SetUsernameAndPassword(username_str,password_str);
            mTcpClient.ConnectToTcpServer(server_address_string,server_port_string);
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
