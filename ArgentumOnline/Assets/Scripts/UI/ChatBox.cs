using System.Collections;
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
    private bool panelView = true;
    private bool close = false;
    private ChatClient mChatClient;

    [SerializeField]
    List<ChatMessage> messageList = new List<ChatMessage>();
    void Start()
    {
        // setup the chat client
        /*GameObject chat_client_object = GameObject.FindGameObjectsWithTag("ChatClient")[0];
        mChatClient = chat_client_object.GetComponent<ChatClient>();
        Debug.Assert(mChatClient!=null);*/
    }

    void Update()
    {
        if (close)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                GameObject mChatDialog = GameObject.Find("ChatDialog");
                Debug.Assert(mChatDialog != null);
                mChatDialog.transform.localScale = new Vector3(0.6f, 0.6f, 0);
                close = false;
            }
            return;
        }
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
            //if (!chatInput.isFocused && Input.GetKeyDown(KeyCode.Return)) { }
                //chatInput.ActivateInputField();
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
        if(messageType == ChatMessage.MessageType.player){
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
            //mChatClient.OnPlayerSays(uuid, sceneName, posx, posy, words);
        }
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
    public void MinimizeMaximize()
    {
        GameObject mChatDialog = GameObject.Find("ChatView");
        Debug.Assert(mChatDialog != null);
        if (panelView)
        {
            mChatDialog.transform.localScale = new Vector3(0, 0, 0);
            panelView = false;
        }
        else
        {
            mChatDialog.transform.localScale = new Vector3(1, 1, 0);
            panelView = true;
        }

    }
    public void Close()
    {
        GameObject mChatDialog = GameObject.Find("ChatDialog");
        Debug.Assert(mChatDialog != null);
        mChatDialog.transform.localScale = new Vector3(0, 0, 0);
        close = true;
    }
    //private ChatClient mChatClient;
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
