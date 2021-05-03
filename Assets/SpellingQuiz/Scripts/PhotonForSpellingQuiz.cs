using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using System.IO;
using System;
using UnityEngine.UI;
/// <summary>
/// This script is to handle chat communication
/// we are sending scroes as a chat so that all the users can see live update.
/// </summary>

public class PhotonForSpellingQuiz : MonoBehaviour, IChatClientListener
{
    public static PhotonForSpellingQuiz instance;
    ChatClient chatClient;
    public bool isSubscribed;
    //public override void OnConnected()
    //{

    //}
    public void DebugReturn(DebugLevel level, string message)
    {
        //debugText.GetComponent<TextMeshProUGUI>().text = debugText.GetComponent<TextMeshProUGUI>().text + "\n" + "Message " + message;
        print(message);
        //throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        //debugText.GetComponent<TextMeshProUGUI>().text = debugText.GetComponent<TextMeshProUGUI>().text + "\n" + "Status " + state;
        print(state.ToString());
        if (state.ToString().Contains("Disconnect"))
        {
            //JoinChat();
        }
        //throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        //debugText.GetComponent<TextMeshProUGUI>().text = debugText.GetComponent<TextMeshProUGUI>().text + "\n" + "Connected!!! ";
        chatClient.Subscribe(new string[] { subscribeTopic }); //subscribe to chat channel once connected to server
    }

    public void OnDisconnected()
    {
        //debugText.GetComponent<TextMeshProUGUI>().text = debugText.GetComponent<TextMeshProUGUI>().text + "\n" + "Disconnected!!! ";
        //throw new System.NotImplementedException();
        JoinChat();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {

        int msgCount = messages.Length;
        for (int i = 0; i < msgCount; i++)
        {

            //go through each received msg
            string sender = senders[i];
            string msg = messages[i].ToString();
            Debug.Log(sender + " : " + msg);
            if (debugText.activeSelf)
            {
                debugText.GetComponent<TextMeshProUGUI>().text = debugText.GetComponent<TextMeshProUGUI>().text + "\n" + msg;
            }

            if (msg.ToLower().Contains("started") && sender.ToLower() != "start" && !DriveTest.instance.isGameStarted)
            {
                if (lobbyPanel.activeSelf)
                {
                    lobbyPanel.SetActive(false);
                }
                DriveTest.instance.StartGame();
                StartCoroutine(HideDebugText());
            }
            if (DriveTest.instance.isGameStarted)
            {
                UpdateScore(sender + ":" + msg);
            }

        }
        //throw new System.NotImplementedException();
    }
    public IEnumerator HideDebugText()
    {
        print("hide debug");
        yield return new WaitForSeconds(5);
        debugText.SetActive(false);
    }
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        print(sender + " : " + message);
        //debugText.GetComponent<TextMeshProUGUI>().text = debugText.GetComponent<TextMeshProUGUI>().text + "\n" + sender + " : " + message;
        //throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //debugText.GetComponent<TextMeshProUGUI>().text = debugText.GetComponent<TextMeshProUGUI>().text + "\n" + "Subscribed: " + subscribeTopic;
        if (!DriveTest.instance.isGameStarted)
        {
            Debug.Log("Subscribed to " + subscribeTopic);
            JoinPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            if (!debugText.activeSelf)
            {
                debugText.SetActive(true);
            }
            //chatPanel.SetActive(true);
            isSubscribed = true;
            SendMessageToAll(userID + " Joined!!!");
            //DriveTest.instance.StartGame();
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        SendMessageToAll(userID + " left!!!");
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        SendMessageToAll(userID + " left!!!");
        //debugText.GetComponent<TextMeshProUGUI>().text = debugText.GetComponent<TextMeshProUGUI>().text + "\n" + user + " joined " + channel;
        //throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        SendMessageToAll(userID + " left!!!");
        throw new System.NotImplementedException();
    }
    public string userID;
    public string subscribeTopic = "somChannel";
    public TMP_InputField UsernameTMP;//, messageTMP, privateUser;
    public GameObject debugText;
    public GameObject JoinPanel, lobbyPanel;//, chatPanel;
    public Button joinBtn;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        JoinPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        //File.WriteAllText(_scoreSavingPath, "");
    }
    public void JoinChat()
    {
        if (userID == "")
        {
            userID = UsernameTMP.text;
        }
#if !UNITY_WEBGL
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(userID));
        //DriveTest.instance.ShowNextOptions();
#else
        //Use BESTHttp Pro for connection
#endif
    }
    //public void SendChatMessage()
    //{
    //    if (privateUser.text == "")
    //    {
    //        chatClient.PublishMessage(subscribeTopic, messageTMP.text);
    //    }
    //    else
    //    {
    //        chatClient.SendPrivateMessage(privateUser.text, messageTMP.text);
    //    }
    //}
    public void SendMessageToAll(string message)
    {
        chatClient.PublishMessage(subscribeTopic, message);
    }
    // Update is called once per frame
    void Update()
    {
        joinBtn.interactable = !string.IsNullOrEmpty(UsernameTMP.text);

        if (chatClient != null)
        {
            chatClient.Service();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            //chatClient.PublishMessage(subscribeTopic, "Som sent this");
            //debugText.GetComponent<TextMeshProUGUI>().text = debugText.GetComponent<TextMeshProUGUI>().text + "\n"+ "ass";
        }
    }
    void OnApplicationQuit()
    {
        if (chatClient != null) { chatClient.Disconnect(); }
        //File.WriteAllText(_scoreSavingPath, "");
    }
    //private string _scoreSavingPath = Application.streamingAssetsPath + "/Scores.txt";
    public List<string> scoreList = new List<string>();
    private void UpdateScore(string message)
    {
        string[] splitMessage = message.Split(':');
        if (scoreList.Count > 0)
        {
            for (int i = 0; i < scoreList.Count; i++)
            {
                if (scoreList[i].Contains(splitMessage[0]))
                {
                    scoreList.Add(message);
                    scoreList.RemoveAt(i);
                    print("contaning " + i + " : " + scoreList.Count);
                    WriteFile();
                    return;
                }
            }
            scoreList.Add(message);
            WriteFile();
        }
        else
        {
            scoreList.Add(message);
            WriteFile();
        }

    }
    private void WriteFile()
    {
        string textToWrite = "";
        //if (File.Exists(_scoreSavingPath))
        //{
        //    if (File.Exists(_scoreSavingPath))
        //    {
        //        File.Delete(_scoreSavingPath);
        //    }
        //    for (int i = 0; i < scoreList.Count; i++)
        //    {
        //        if (textToWrite == "")
        //        {
        //            textToWrite = scoreList[i];
        //        }
        //        else
        //        {
        //            textToWrite = textToWrite + "\n" + scoreList[i];
        //        }
        //        //using (StreamWriter writer = new StreamWriter(_scoreSavingPath))
        //        //{
        //        //    writer.WriteLine(scoreList[i]);
        //        //}
        //    }
        //}
        //else
        //{
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (textToWrite == "")
            {
                textToWrite = scoreList[i];
            }
            else
            {
                textToWrite = textToWrite + "\n" + scoreList[i];
            }
        }
        //}
        DriveTest.instance.scores.GetComponent<TextMeshProUGUI>().text = textToWrite;
    }

}
