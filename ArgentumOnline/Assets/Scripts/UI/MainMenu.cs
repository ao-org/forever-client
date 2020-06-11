/*
		Argentum Forever - Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
		gulfas@gmail.com
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
using System.Linq;

public class MainMenu : MonoBehaviour
{
    private EventSystem mEventSystem;
    public LocalizedString LoginErrText_MSGBOX_TITLE;
    public LocalizedString SignupErrText_MSGBOX_TITLE;
    public LocalizedString LoginErrText_USER_ALREADY_HOLDS_ACTIVE_SESSION;
    public LocalizedString LoginErrText_ACCOUNT_DOESNT_EXIST;
    public LocalizedString LoginErrText_MUST_ACTIVATE_ACCOUNT;

    public LocalizedString LoginErrText_LOGIN_ERROR_WRONG_PASSWORD;
    public LocalizedString SignupErrText_ACCOUNT_ALREADY_EXIST;
    public LocalizedString InputErrText_INPUT_ERROR_INVALID_PASSWORD;
    public LocalizedString InputErrText_INPUT_ERROR_INVALID_CONFIRM_PASSWORD;
    public LocalizedString InputErrText_INPUT_ERROR_INVALID_USERNAME;
    public LocalizedString InputErrText_INPUT_ERROR_INVALID_ACTIVATION_CODE;
    public LocalizedString InputErrText_INPUT_ERROR_TITLE;
    public LocalizedString ConnectionErrText_CONNECTION_ERROR_MSGBOX_TITLE;
    public LocalizedString ConnectionErrText_CONNECTION_ERROR_CANNOT_REACH_SERVER;

    public LocalizedString SignupErrText_PASSWORD_TOO_SHORT;
    public LocalizedString SignupErrText_PASSWORD_TOO_LONG;
    public LocalizedString SignupErrText_PASSWORD_IS_NOT_ALNUM;
    public LocalizedString SignupErrText_USERNAME_TOO_SHORT;
    public LocalizedString SignupErrText_USERNAME_TOO_LONG;
    public LocalizedString SignupErrText_USERNAME_IS_NOT_ALNUM;
    public LocalizedString SignupErrText_PASSWORD_CANNOT_CONTAIN_USERNAME;
    public LocalizedString SignupErrText_USERNAME_CANNOT_START_WITH_NUMBER;
    public LocalizedString SignupErrText_PASSWORD_MUST_HAVE_ONE_UPPERCASE;
    public LocalizedString SignupErrText_PASSWORD_MUST_HAVE_ONE_LOWERCASE;
    public LocalizedString SignupErrText_PASSWORD_MUST_HAVE_TWO_NUMBERS;
    public LocalizedString SignupErrText_INVALID_EMAIL;
    public LocalizedString SignupErrText_INVALID_CONFIRM_EMAIL;
    public LocalizedString SignupErrText_INVALID_FIRST_LAST_NAME;
    public LocalizedString SignupErrText_TERMS_NOT_ACCEPTED;

    public LocalizedString ActivateOkayText_ACTIVATE_OKAY;
    public LocalizedString ActivateErrText_ACTIVATE_ERROR_INVALID_CODE;
    public LocalizedString ActivateErrText_ACTIVATE_MSG_BOX_TITLE;

    public LocalizedString SignupText_TERMS_CONDITIONS_TITLE;
    public LocalizedString SignupText_TERMS_CONDITIONS_TEXT;

    private void CreateAndInitLocalizedStrings(){
        mLocalizedStringMappings = new Dictionary<string,LocalizedString>();
        mLocalizedStringMappings["LOGIN_ERROR_MSG_BOX_TITLE"]= LoginErrText_MSGBOX_TITLE;
        mLocalizedStringMappings["SIGNUP_ERROR_MSG_BOX_TITLE"]= SignupErrText_MSGBOX_TITLE;
        mLocalizedStringMappings["USER_ALREADY_HOLDS_ACTIVE_SESSION"]= LoginErrText_USER_ALREADY_HOLDS_ACTIVE_SESSION;
        mLocalizedStringMappings["ACCOUNT_DOESNT_EXIST"]= LoginErrText_ACCOUNT_DOESNT_EXIST;
        mLocalizedStringMappings["ACCOUNT_ALREADY_EXISTS"]= SignupErrText_ACCOUNT_ALREADY_EXIST;
        mLocalizedStringMappings["INPUT_ERROR_INVALID_PASSWORD"]= InputErrText_INPUT_ERROR_INVALID_PASSWORD;
        mLocalizedStringMappings["INPUT_ERROR_INVALID_CONFIRM_PASSWORD"] = InputErrText_INPUT_ERROR_INVALID_CONFIRM_PASSWORD;
        mLocalizedStringMappings["INPUT_ERROR_INVALID_USER"]= InputErrText_INPUT_ERROR_INVALID_USERNAME;
        mLocalizedStringMappings["INPUT_ERROR_TITLE"]= InputErrText_INPUT_ERROR_TITLE;
        mLocalizedStringMappings["CONNECTION_ERROR_MSGBOX_TITLE"]= ConnectionErrText_CONNECTION_ERROR_MSGBOX_TITLE;
        mLocalizedStringMappings["CONNECTION_ERROR_CANNOT_REACH_SERVER"]= ConnectionErrText_CONNECTION_ERROR_CANNOT_REACH_SERVER;
        mLocalizedStringMappings["PASSWORD_TOO_SHORT"]= SignupErrText_PASSWORD_TOO_SHORT;
        mLocalizedStringMappings["PASSWORD_TOO_LONG"]= SignupErrText_PASSWORD_TOO_LONG;
        mLocalizedStringMappings["PASSWORD_IS_NOT_ALNUM"]= SignupErrText_PASSWORD_IS_NOT_ALNUM;
        mLocalizedStringMappings["USERNAME_TOO_SHORT"]= SignupErrText_USERNAME_TOO_SHORT;
        mLocalizedStringMappings["USERNAME_TOO_LONG"]= SignupErrText_USERNAME_TOO_LONG;
        mLocalizedStringMappings["USERNAME_IS_NOT_ALNUM"]= SignupErrText_USERNAME_IS_NOT_ALNUM;
        mLocalizedStringMappings["PASSWORD_CANNOT_CONTAIN_USERNAME"]= SignupErrText_PASSWORD_CANNOT_CONTAIN_USERNAME;
        mLocalizedStringMappings["USERNAME_CANNOT_START_WITH_NUMBER"]= SignupErrText_USERNAME_CANNOT_START_WITH_NUMBER;
        mLocalizedStringMappings["PASSWORD_MUST_HAVE_ONE_UPPERCASE"]= SignupErrText_PASSWORD_MUST_HAVE_ONE_UPPERCASE;
        mLocalizedStringMappings["PASSWORD_MUST_HAVE_ONE_LOWERCASE"]= SignupErrText_PASSWORD_MUST_HAVE_ONE_LOWERCASE;
        mLocalizedStringMappings["PASSWORD_MUST_HAVE_TWO_NUMBERS"]= SignupErrText_PASSWORD_MUST_HAVE_TWO_NUMBERS;
        mLocalizedStringMappings["INVALID_EMAIL"]= SignupErrText_INVALID_EMAIL;
        mLocalizedStringMappings["INVALID_CONFIRM_EMAIL"] = SignupErrText_INVALID_CONFIRM_EMAIL;
        mLocalizedStringMappings["INVALID_FIRST_LAST_NAME"] = SignupErrText_INVALID_FIRST_LAST_NAME;
        mLocalizedStringMappings["TERMS_NOT_ACCEPTED"] = SignupErrText_TERMS_NOT_ACCEPTED;
        mLocalizedStringMappings["MUST_ACTIVATE_ACCOUNT"]= LoginErrText_MUST_ACTIVATE_ACCOUNT;
        mLocalizedStringMappings["INPUT_ERROR_INVALID_ACTIVATION_CODE"]= InputErrText_INPUT_ERROR_INVALID_ACTIVATION_CODE;
        mLocalizedStringMappings["ACTIVATE_OKAY"]= ActivateOkayText_ACTIVATE_OKAY;
        mLocalizedStringMappings["ACTIVATE_ERROR_INVALID_CODE"]= ActivateErrText_ACTIVATE_ERROR_INVALID_CODE;
        mLocalizedStringMappings["ACTIVATE_MSG_BOX_TITLE"]= ActivateErrText_ACTIVATE_MSG_BOX_TITLE;
        mLocalizedStringMappings["WRONG_PASSWORD"]= LoginErrText_LOGIN_ERROR_WRONG_PASSWORD;
        //Ver si se hace por pdf, web o server text
        mLocalizedStringMappings["TERMS_CONDITIONS_TITLE"] = SignupText_TERMS_CONDITIONS_TITLE;
        mLocalizedStringMappings["TERMS_CONDITIONS_TEXT"] = SignupText_TERMS_CONDITIONS_TEXT;
    }
    public void OnRegisterButtonClicked(){
        Debug.Log("OnRegisterButtonClicked");
        mSignupDialog.transform.localScale = new Vector3(1, 1, 1);
    }
    public void OnApplicationQuit(){
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
    public void OnAccountCreated(){
        Debug.Log("AccountCreated");
        mActivateDialog.transform.localScale = new Vector3(1, 1, 1);
    }
    public void OnLoginOkay(){
        Debug.Log("LOGIN_OKAY");
        //At this point the LoginClient logged into the user's account and we hold a valid session Token
        InputField server_address_input = GameObject.Find("ServerIPInputField").GetComponent<InputField>();
        Debug.Assert(server_address_input!=null);
        string server_address_string    = server_address_input.text;
        string server_port_string       = "6000";

        try {
          //Attempt to connect to game Server
              Debug.Log("World Server address: " + server_address_string + ":" + server_port_string);
              //mWorldClient.SetUsernameAndPassword("morgolock","Pablo17");
              mWorldClient.ConnectToTcpServer(server_address_string,server_port_string);
        }
        catch (Exception e){
  			     Debug.Log("Failed to connect to server " + e);
        }


    }
    public void OnAccountActivated(){
        Debug.Log("AccountActivated");
        mActivateDialog.transform.localScale = new Vector3(0, 0, 0);
        mSignupDialog.transform.localScale = new Vector3(0, 0, 0);
        this.ShowMessageBox("ACTIVATE_MSG_BOX_TITLE","ACTIVATE_OKAY",true);
    }
    public void OnActivationCanceled(){
        Debug.Log("AccountCreated");
        mActivateDialog.transform.localScale = new Vector3(0, 0, 0);
        mSignupDialog.transform.localScale = new Vector3(0, 0, 0);
    }
    public void OnSignupCanceled(){
        Debug.Log("OnSignupCanceled");
        mSignupDialog.transform.localScale = new Vector3(0, 0, 0);
    }
    public void OnSendCodeButton(){
        Debug.Log("OnSendCodeButton");
        InputField server_address_input = GameObject.Find("ServerIPInputField").GetComponent<InputField>();
        InputField server_port_input    = GameObject.Find("ServerPortInputField").GetComponent<InputField>();
        //InputField username_input       = GameObject.Find("UsernameInputField").GetComponent<InputField>();
        //InputField password_input       = GameObject.Find("PasswordInputField").GetComponent<InputField>();
        InputField code_input           = GameObject.Find("ActivateCodeInputField").GetComponent<InputField>();

        Debug.Assert(server_address_input!=null);
        Debug.Assert(server_port_input!=null);
        //Debug.Assert(username_input!=null);
        //Debug.Assert(password_input!=null);
        Debug.Assert(code_input!=null);
        //string username_str             = username_input.text;
        //string password_str             = password_input.text;
        string server_address_string    = server_address_input.text;
        string server_port_string       = server_port_input.text;
        string code_string              = code_input.text;

        if(code_string == null || code_string.Length<8){
            this.ShowMessageBox("INPUT_ERROR_TITLE","INPUT_ERROR_INVALID_ACTIVATION_CODE",true);
            return;
        }
        try {
          //Attempt to connect to game Server
          if( mLoginClient.IsConnected()){
              mLoginClient.AttemptToActivate();
          }
          else {
              Debug.Log("Server address: " + server_address_string + ":" + server_port_string);
              //mLoginClient.SetUsernameAndPassword(username_str,password_str);
              // username and password already setup in the signup flow
              mLoginClient.SetActivationCode(code_string);
              mLoginClient.ConnectToTcpServer(server_address_string,server_port_string,"ACTIVATE_REQUEST");
          }
        }
        catch (Exception e){
                Debug.Log("Failed to connect to server " + e);
        }
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
                next =  Selectable.allSelectables[0];
                mEventSystem.SetSelectedGameObject(next.gameObject);
            }
        }
    }
    private LoginClient mLoginClient;
    private WorldClient mWorldClient;
    private GameObject mMessageBox;
    private GameObject mSignupDialog;
    private GameObject mActivateDialog;
    private GameObject mToggle;
    private void Start()
    {
        CreateAndInitLocalizedStrings();
        mEventSystem = EventSystem.current;
        // setup the login client
        GameObject login_client_object = GameObject.FindGameObjectsWithTag("LoginClient")[0];
        mLoginClient = login_client_object.GetComponent<LoginClient>();
        mLoginClient.SetMainMenu(this);
        // setup the world client
        GameObject world_client_object = GameObject.FindGameObjectsWithTag("WorldClient")[0];
        mWorldClient = world_client_object.GetComponent<WorldClient>();
        mWorldClient.SetMainMenu(this);

        mMessageBox = GameObject.Find("MessageBox");
        Debug.Assert(mMessageBox!=null);
        mMessageBox.transform.localScale = new Vector3(0, 0, 0);

        mSignupDialog = GameObject.Find("SignupDialog");
        Debug.Assert(mSignupDialog!=null);
        //         mSignupDialog.transform.localScale = new Vector3(0, 0, 0);
        mToggle = GameObject.Find("Toggle");

        mActivateDialog = GameObject.Find("ActivateDialog");
        Debug.Assert(mActivateDialog!=null);
        mActivateDialog.transform.localScale = new Vector3(0, 0, 0);
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
            Debug.Assert(mLocalizedStringMappings.ContainsKey(title), "Missing the Localized String in the Dictionary");
            Debug.Assert(mLocalizedStringMappings.ContainsKey(text) , "Missing the Localized String in the Dictionary");
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
        InputField signup_confirm_password_input    = GameObject.Find("SignUpConfirmPasswordInputField").GetComponent<InputField>();
        InputField signup_first_name_input  = GameObject.Find("SignUpFirstNameInputField").GetComponent<InputField>();
        InputField signup_last_name_input   = GameObject.Find("SignUpLastNameInputField").GetComponent<InputField>();
        InputField signup_email_input       = GameObject.Find("SignUpEmailInputField").GetComponent<InputField>();
        InputField signup_confirm_email_input       = GameObject.Find("SignUpConfirmEmailInputField").GetComponent<InputField>();
        InputField signup_dob_input         = GameObject.Find("SignUpDOBInputField").GetComponent<InputField>();
        InputField signup_pob_input         = GameObject.Find("SignUpPOBInputField").GetComponent<InputField>();
        InputField signup_secretq1_input    = GameObject.Find("SignUpSecretQ1InputField").GetComponent<InputField>();
        InputField signup_secretq2_input    = GameObject.Find("SignUpSecretQ2InputField").GetComponent<InputField>();
        InputField signup_secreta1_input    = GameObject.Find("SignUpSecretA1InputField").GetComponent<InputField>();
        InputField signup_secreta2_input    = GameObject.Find("SignUpSecretA2InputField").GetComponent<InputField>();
        InputField signup_mobile_input      = GameObject.Find("SignUpMobileInputField").GetComponent<InputField>();
        InputField server_address_input     = GameObject.Find("ServerIPInputField").GetComponent<InputField>();
        InputField server_port_input        = GameObject.Find("ServerPortInputField").GetComponent<InputField>();
        Dropdown signup_language_dropdown   = GameObject.Find("SignUpLanguageDropdown").GetComponent<Dropdown>();
        Debug.Assert(signup_email_input != null);
        Debug.Assert(signup_confirm_email_input != null);
        Debug.Assert(signup_username_input!=null);
        Debug.Assert(signup_password_input!=null);
        Debug.Assert(signup_confirm_password_input != null);
        Debug.Assert(signup_first_name_input!=null);
        Debug.Assert(signup_last_name_input!=null);
        Debug.Assert(signup_language_dropdown!=null);
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
        string confirm_password_str     = signup_confirm_password_input.text;
        string server_address_string    = server_address_input.text;
        string server_port_string       = server_port_input.text;
        string first_name_string        = signup_first_name_input.text;
        string last_name_string         = signup_last_name_input.text;
        string email_string             = signup_email_input.text;
        string confirm_email_string     = signup_confirm_email_input.text;
        string dob_string               = signup_dob_input.text;
        string pob_string               = signup_pob_input.text;
        string secretq1_string          = signup_secretq1_input.text;
        string secretq2_string          = signup_secretq2_input.text;
        string secreta2_string          = signup_secreta1_input.text;
        string secreta1_string          = signup_secreta2_input.text;
        string mobile_string            = signup_mobile_input.text;
        //string language_string          = signup_language_input.text;
        var drop_value = signup_language_dropdown.value;
        //Change the message to say the name of the current Dropdown selection using the value
        string language_string = signup_language_dropdown.options[drop_value].text;
        if (email_string == null || !email_string.Contains("@"))
        {
            this.ShowMessageBox("INPUT_ERROR_TITLE", "INVALID_EMAIL", true);
            return;
        }
        if (confirm_email_string == null || (email_string != confirm_email_string))
        {
            //this.ShowMessageBox("INPUT_ERROR_TITLE", "INVALID_CONFIRM_EMAIL", true);
            this.ShowMessageBox("INPUT_ERROR_TITLE", "INVALID_CONFIRM_EMAIL", true);
            return;
        }
        if (username_str == null || username_str.Length<3){
            this.ShowMessageBox("INPUT_ERROR_TITLE","INPUT_ERROR_INVALID_USER",true);
            return;
        }
        if(password_str == null || password_str.Length<6 || !password_str.All(char.IsLetterOrDigit))
        {
            this.ShowMessageBox("INPUT_ERROR_TITLE","INPUT_ERROR_INVALID_PASSWORD",true);
            return;
        }
        if (confirm_password_str == null || confirm_password_str != password_str)
        {
            this.ShowMessageBox("INPUT_ERROR_TITLE", "INPUT_ERROR_INVALID_CONFIRM_PASSWORD", true);
            return;
        }
        if (first_name_string == null || last_name_string == null)
        {
            this.ShowMessageBox("INPUT_ERROR_TITLE", "INVALID_FIRST_LAST_NAME", true);
            return;
        }
        if (mobile_string == null)
        {
            this.ShowMessageBox("INPUT_ERROR_TITLE", "INVALID_FIRST_LAST_NAME", true);
            return;
        }
        if(!mToggle.GetComponent<Toggle>().isOn)
        {
            this.ShowMessageBox("INPUT_ERROR_TITLE", "TERMS_NOT_ACCEPTED", true);
            return;
        }

        try {
            mLoginClient.SetUsernameAndPassword(username_str,password_str);

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
                { "LANGUAGE", language_string },
                { "SECRETQ1", secretq1_string },
                { "SECRETQ2", secretq2_string },
                { "SECRETA2", secreta2_string },
                { "SECRETA1", secreta1_string },
            };
            mLoginClient.SetSignupData(signup_data);
            //Attempt to connect to game Server
            if( mLoginClient.IsConnected()){
                mLoginClient.AttemptToSignup();
            }
            else {
                Debug.Log("Server address: " + server_address_string + ":" + server_port_string);
                mLoginClient.ConnectToTcpServer(server_address_string,server_port_string,"SIGNUP_REQUEST");
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
        if( mLoginClient.IsConnected()){
            mLoginClient.AttemptToLogin();
        }
        else {
            Debug.Log("Server address: " + server_address_string + ":" + server_port_string);
            mLoginClient.SetUsernameAndPassword(username_str,password_str);
            mLoginClient.ConnectToTcpServer(server_address_string,server_port_string,"LOGIN_REQUEST");
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
    public void ShowTermsAndConditions()
    {
        Debug.Log("ShowTermsAndConditions");
        this.ShowMessageBox("TERMS_CONDITIONS_TITLE", "TERMS_CONDITIONS_TEXT", true);
    }
    public void ResendActivationCode()
    {
        Debug.Log("public void ResendActivationCode()");
        InputField server_address_input = GameObject.Find("ServerIPInputField").GetComponent<InputField>();
        InputField server_port_input = GameObject.Find("ServerPortInputField").GetComponent<InputField>();
        InputField username_input = GameObject.Find("SignUpUsernameInputField").GetComponent<InputField>();
        InputField email_input = GameObject.Find("SignUpEmailInputField").GetComponent<InputField>();
        Debug.Assert(server_address_input != null);
        Debug.Assert(server_port_input != null);
        Debug.Assert(username_input != null);
        Debug.Assert(email_input != null);
        string username_str = username_input.text;
        string email_str = email_input.text;
        string server_address_string = server_address_input.text;
        string server_port_string = server_port_input.text;

        try
        {
            //Attempt to connect to game Server
            if (mLoginClient.IsConnected())
            {
                mLoginClient.AttemptToReSendCode();
            }
            else
            {
                Debug.Log("Server address: " + server_address_string + ":" + server_port_string);
                mLoginClient.ConnectToTcpServer(server_address_string, server_port_string, "CODE_REQUEST");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed to connect to server " + e);
        }
    }
}
