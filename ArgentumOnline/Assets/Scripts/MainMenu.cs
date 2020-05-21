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
    public LocalizedString InputErrText_INPUT_ERROR_INVALID_PASSWORD;
    public LocalizedString InputErrText_INPUT_ERROR_INVALID_USERNAME;
    public LocalizedString InputErrText_INPUT_ERROR_TITLE;
    public LocalizedString ConnectionErrText_CONNECTION_ERROR_MSGBOX_TITLE;
    public LocalizedString ConnectionErrText_CONNECTION_ERROR_CANNOT_REACH_SERVER;

    private void CreateAndInitLocalizedStrings(){
        mLocalizedStringMappings = new Dictionary<string,LocalizedString>();
        mLocalizedStringMappings["LOGIN_ERROR_MSG_BOX_TITLE"]= LoginErrText_MSGBOX_TITLE;
        mLocalizedStringMappings["USER_ALREADY_HOLDS_ACTIVE_SESSION"]= LoginErrText_USER_ALREADY_HOLDS_ACTIVE_SESSION;
        mLocalizedStringMappings["ACCOUNT_DOESNT_EXIST"]= LoginErrText_ACCOUNT_DOESNT_EXIST;
        mLocalizedStringMappings["INPUT_ERROR_INVALID_PASSWORD"]= InputErrText_INPUT_ERROR_INVALID_PASSWORD;
        mLocalizedStringMappings["INPUT_ERROR_INVALID_USER"]= InputErrText_INPUT_ERROR_INVALID_USERNAME;
        mLocalizedStringMappings["INPUT_ERROR_TITLE"]= InputErrText_INPUT_ERROR_TITLE;
        mLocalizedStringMappings["CONNECTION_ERROR_MSGBOX_TITLE"]= ConnectionErrText_CONNECTION_ERROR_MSGBOX_TITLE;
        mLocalizedStringMappings["CONNECTION_ERROR_CANNOT_REACH_SERVER"]= ConnectionErrText_CONNECTION_ERROR_CANNOT_REACH_SERVER;
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
    private GameObject mMessageBox;
    private void Start()
    {
         CreateAndInitLocalizedStrings();
         mEventSystem = EventSystem.current;
         GameObject tcp_client_object = GameObject.FindGameObjectsWithTag("TCPClient")[0];
         mTcpClient = tcp_client_object.GetComponent<TCPClient>();
         mTcpClient.SetMainMenu(this);
         mMessageBox = GameObject.Find("MessageBox");
         Debug.Assert(mMessageBox!=null);
         mMessageBox.transform.localScale = new Vector3(0, 0, 0);
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
        mMessageBox.transform.localScale = new Vector3(1f, 1f, 1f);
    }
    public void CreateAccount(){
        Debug.Log("CreateAccount");
        InputField signup_username_input    = GameObject.Find("SignUpUsernameInputField").GetComponent<InputField>();
        InputField signup_password_input    = GameObject.Find("SignUpPasswordInputField").GetComponent<InputField>();
        InputField signup_first_name_input  = GameObject.Find("SignUpFirstNameInputField").GetComponent<InputField>();
        InputField signup_last_name_input   = GameObject.Find("SignUpLastNameInputField").GetComponent<InputField>();
        InputField signup_email_input       = GameObject.Find("SignUpEmailInputField").GetComponent<InputField>();
        InputField signup_dob_input         = GameObject.Find("SignUpDOBInputField").GetComponent<InputField>();
        InputField signup_pob_input         = GameObject.Find("SignUpPOBInputField").GetComponent<InputField>();
        InputField signup_secretq1_input    = GameObject.Find("SignUpSecretQ1InputField").GetComponent<InputField>();
        InputField signup_secretq2_input    = GameObject.Find("SignUpSecretQ2InputField").GetComponent<InputField>();
        InputField signup_secreta1_input    = GameObject.Find("SignUpSecretA1InputField").GetComponent<InputField>();
        InputField signup_secreta2_input    = GameObject.Find("SignUpSecretA2InputField").GetComponent<InputField>();
        InputField signup_mobile_input      = GameObject.Find("SignUpMobileInputField").GetComponent<InputField>();
        InputField server_address_input     = GameObject.Find("ServerIPInputField").GetComponent<InputField>();
        InputField server_port_input        = GameObject.Find("ServerPortInputField").GetComponent<InputField>();
        Debug.Assert(signup_username_input!=null);
        Debug.Assert(signup_password_input!=null);
        Debug.Assert(signup_first_name_input!=null);
        Debug.Assert(signup_last_name_input!=null);
        Debug.Assert(signup_email_input!=null);
        Debug.Assert(signup_dob_input!=null);
        Debug.Assert(signup_pob_input!=null);
        Debug.Assert(signup_secretq1_input!=null);
        Debug.Assert(signup_secretq2_input!=null);
        Debug.Assert(signup_secreta2_input!=null);
        Debug.Assert(signup_secreta1_input!=null);
        Debug.Assert(signup_mobile_input!=null);
        Debug.Assert(server_address_input!=null);
        Debug.Assert(server_port_input!=null);
        string username_str             = signup_username_input.text;
        string password_str             = signup_password_input.text;
        string server_address_string    = server_address_input.text;
        string server_port_string       = server_port_input.text;
        string first_name_string        = signup_first_name_input.text;
        string last_name_string         = signup_last_name_input.text;
        string email_string             = signup_email_input.text;
        string dob_string               = signup_dob_input.text;
        string pob_string               = signup_pob_input.text;
        string secretq1_string          = signup_secretq1_input.text;
        string secretq2_string          = signup_secretq2_input.text;
        string secreta2_string          = signup_secreta1_input.text;
        string secreta1_string          = signup_secreta2_input.text;
        string mobile_string            = signup_mobile_input.text;

        if(username_str == null || username_str.Length<3){
            this.ShowMessageBox("INPUT_ERROR_TITLE","INPUT_ERROR_INVALID_USER",true);
            return;
        }
        if(password_str == null || password_str.Length<3){
            this.ShowMessageBox("INPUT_ERROR_TITLE","INPUT_ERROR_INVALID_PASSWORD",true);
            return;
        }

        try {
            mTcpClient.SetUsernameAndPassword(username_str,password_str);

            Dictionary<string, string> signup_data = new Dictionary<string,string>
            {
                { "USERNAME", username_str },
        		{ "PASSWORD", password_str },
        		{ "FIRST_NAME", first_name_string },
        		{ "LAST_NAME", last_name_string },
                { "EMAIL", email_string },
                { "DOB", dob_string },
                { "POB", pob_string },
                { "MOBILE", mobile_string },
                { "SECRETQ1", secretq1_string },
                { "SECRETQ2", secretq2_string },
                { "SECRETA2", secreta2_string },
                { "SECRETA1", secreta1_string },
            };
            mTcpClient.SetSignupData(signup_data);
            //Attempt to connect to game Server
            if( mTcpClient.IsConnected()){
                mTcpClient.AttemptToSignup();
            }
            else {
                Debug.Log("Server address: " + server_address_string + ":" + server_port_string);
                mTcpClient.ConnectToTcpServer(server_address_string,server_port_string,"SIGNUP_REQUEST");
            }
        }
        catch (Exception e){
                   Debug.Log("Failed to connect to server " + e);
        }
    }

    public void PlayGame(){
      InputField server_address_input = GameObject.Find("ServerIPInputField").GetComponent<InputField>();
      InputField server_port_input    = GameObject.Find("ServerPortInputField").GetComponent<InputField>();
      InputField username_input       = GameObject.Find("UsernameInputField").GetComponent<InputField>();
      InputField password_input       = GameObject.Find("PasswordInputField").GetComponent<InputField>();
      Debug.Assert(server_address_input!=null);
      Debug.Assert(server_port_input!=null);
      Debug.Assert(username_input!=null);
      Debug.Assert(password_input!=null);
      string username_str             = username_input.text;
      string password_str             = password_input.text;
      string server_address_string    = server_address_input.text;
      string server_port_string       = server_port_input.text;

      if(username_str == null || username_str.Length<3){
          this.ShowMessageBox("INPUT_ERROR_TITLE","INPUT_ERROR_INVALID_USER",true);
          return;
      }
      if(password_str == null || password_str.Length<3){
          this.ShowMessageBox("INPUT_ERROR_TITLE","INPUT_ERROR_INVALID_PASSWORD",true);
          return;
      }

      try {
        //Attempt to connect to game Server
        if( mTcpClient.IsConnected()){
            mTcpClient.AttemptToLogin();
        }
        else {
            Debug.Log("Server address: " + server_address_string + ":" + server_port_string);
            mTcpClient.SetUsernameAndPassword(username_str,password_str);
            mTcpClient.ConnectToTcpServer(server_address_string,server_port_string,"LOGIN_REQUEST");
        }
      }
      catch (Exception e){
			     Debug.Log("Failed to connect to server " + e);
      }
    }
    public void QuitGame(){
        Debug.Log("QuitGame");
        Application.Quit();
    }
}
