using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
