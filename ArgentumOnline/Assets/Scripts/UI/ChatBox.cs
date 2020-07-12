﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChatBox : MonoBehaviour
{
    public int maxMessages = 25;
    public GameObject chatPanel, textObject;
    public InputField chatInput;
    public Color playerColorMsg, infoColorMsg, systemColorMsg;

    [SerializeField]
    List<ChatMessage> messageList = new List<ChatMessage>();
    void Start()
    {
        // setup the chat client
        GameObject chat_client_object = GameObject.FindGameObjectsWithTag("ChatClient")[0];
        mChatClient = chat_client_object.GetComponent<ChatClient>();
        Debug.Assert(mChatClient!=null);
    }
    static int inc = 0;
    void Update()
    {
        if (chatInput.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChatBox(chatInput.text);
                chatInput.text = "";
            }
        }
        else
        {
            if (!chatInput.isFocused && Input.GetKeyDown(KeyCode.Return))
                chatInput.ActivateInputField();
        }
        if (!chatInput.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Key Space");
                SendMessageToChatBox("*********Space Key Pressed!****** " + inc.ToString());
                inc++;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Key Space");
            SendMessageToChatBox("*********Space Key Pressed!****** " + inc.ToString(), ChatMessage.MessageType.system);
            inc++;
        }
    }

    public void SendMessageToChatBox(string text, ChatMessage.MessageType messageType = ChatMessage.MessageType.player)
    {
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }
        ChatMessage newMessage = new ChatMessage();
        newMessage.text = text;
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);
        messageList.Add(newMessage);
        Debug.Assert(mChatClient!=null);
        GameObject player_object = GameObject.FindGameObjectsWithTag("Player")[0];
        Debug.Assert(player_object!=null);
        var  char_pos = player_object.transform.position;
        var  words = newMessage.text;
        var active_scene = SceneManager.GetActiveScene();
        var sceneName = active_scene.name;
        var posx = char_pos.x;
        var posy = char_pos.y;
        var uuid = player_object.name;
        Debug.Log("ChatBox: " + words + " " + name + " " + posx.ToString() + " " + sceneName);
        mChatClient.OnPlayerSays(uuid, sceneName, posx, posy, words);
    }

    Color MessageTypeColor(ChatMessage.MessageType messagetype)
    {
        Color color = playerColorMsg;
        switch(messagetype)
        {
            case ChatMessage.MessageType.info:
                color = infoColorMsg;
                break;
            case ChatMessage.MessageType.system:
                color = systemColorMsg;
                break;
        }
        return color;
    }
    private ChatClient mChatClient;
}



[System.Serializable]
public class ChatMessage
{
    public string text;
    public Text textObject;
    public MessageType messageType;

    public enum MessageType
    {
        player,
        info,
        system
    }
}
