#if !UNITY_WEBGL
using BestHTTP;
using BestHTTP.WebSocket;
#endif
using System;
using TMPro;
using UnityEngine;

public enum WebSocket_Connection_State
{
    Connecting = 0,
    Open = 1,
    Closing = 2,
    Closed = 3,
    Ready = 4
}

class WebSocketToHCP
{
#if !UNITY_WEBGL
    private WebSocket ws;
#endif
    private String _url = "";
    private String _proxy = "";
    private bool _timeoutEnabled = false;
    private TimeSpan _connectTimeout = new TimeSpan(0, 0, 0);
    private string _subscribeTopic;
    public delegate void MessageEventHandler(string message);
    public event MessageEventHandler onMessage;
    private WebSocket_Connection_State _connectionState;
    public WebSocket_Connection_State ConnectionState
    {
        get
        {
#if UNITY_WEBGL
            //Closed - 3
            //Closing - 2
            //Connecting - 0
            //Open - 1

            int state = Calls.CST_WebGL_GetWebSocketConnectionReadyState();

            if (state == 0)
            {
                return WebSocket_Connection_State.Connecting;
            }
            else if (state == 1)
            {
                return WebSocket_Connection_State.Open;
            }
            else if (state == 2)
            {
                return WebSocket_Connection_State.Closing;
            }
            else if (state == 3)
            {
                return WebSocket_Connection_State.Closed;
            }
            else
            {
                return WebSocket_Connection_State.Ready;
            }
#else
            if (_connectionState == WebSocket_Connection_State.Ready)
                return _connectionState;
            else if (ws.State == WebSocketStates.Closed)
                return WebSocket_Connection_State.Closed;
            else if (ws.State == WebSocketStates.Connecting)
                return WebSocket_Connection_State.Connecting;
            else if (ws.State == WebSocketStates.Open)
                return WebSocket_Connection_State.Open;
            else if (ws.State == WebSocketStates.Closing)
                return WebSocket_Connection_State.Closing;
            else
                return _connectionState;
#endif
        }
    }
    public void CreateNewInstance()
    {
#if UNITY_WEBGL
        if (string.IsNullOrWhiteSpace(_subscribeTopic) == false)
        {
            Calls.CST_WebGL_CustomWebSocketConfig_SubscribeTopics("[\"" + _subscribeTopic + "\"]");
        }
#else
        ws = new WebSocket(new Uri(_url));
        if (_proxy != "")
        {
            ws.InternalRequest.Proxy = new HTTPProxy(new Uri("http://" + _proxy));
        }

        if (_timeoutEnabled)
        {
            ws.InternalRequest.ConnectTimeout = _connectTimeout;
        }

        //Adding Handlers
        ws.OnOpen += OnOpenHandler;
        ws.OnMessage += OnMessageHandler;
        ws.OnClosed += OnCloseHandler;
        ws.OnError += OnErrorHandler;
        ws.OnErrorDesc += OnErrorDescHandler;
#endif

        _connectionState = WebSocket_Connection_State.Ready;
    }
    public WebSocketToHCP(string URL, string proxy)
    {
        _url = URL;
        _proxy = proxy;
        CreateNewInstance();
    }

    public WebSocketToHCP(string URL, int timeout, string proxy)
    {
        _url = URL;
        _proxy = proxy;
        _timeoutEnabled = true;
        _connectTimeout = new TimeSpan(0, 0, timeout);
        CreateNewInstance();
    }

    public WebSocketToHCP(string URL, int timeout, string proxy, string SubscribeTopic)
        : this(URL, timeout, proxy)
    {
        _url = URL;
        _proxy = proxy;
        _timeoutEnabled = true;
        _connectTimeout = new TimeSpan(0, 0, timeout);
        _subscribeTopic = SubscribeTopic;
        CreateNewInstance();
    }
    private void OnOpenHandler(object sender)
    {
        _connectionState = WebSocket_Connection_State.Open;
        Debug.Log(_connectionState);
        subscribeTopic(_subscribeTopic);
    }

    private void OnMessageHandler(object sender, string m)
    {
        Debug.Log(m);
        onMessage(m);
    }

#if !UNITY_WEBGL

    private void OnErrorHandler(object sender, Exception ex)
    {
        Debug.Log("Error");
        Debug.Log(ws.State);
        Debug.Log(ex);
        ws.Close();
    }

    public void OnErrorDescHandler(WebSocket webSocket, string reason)
    {
        Debug.Log(ws.State);
        Debug.Log(reason);
        ws.Close();
    }

    private void OnCloseHandler(object sender, UInt16 code, string message)
    {
        _connectionState = WebSocket_Connection_State.Closed;
        Debug.Log("Close");
        Debug.Log(message);
        Debug.Log(code);
    }

#endif

    public void ConnectToHCP()
    {
        _connectionState = WebSocket_Connection_State.Connecting;
#if UNITY_WEBGL
        Calls.CST_WebGL_OpenWebSocketConnection();
#else
        ws.Open();
#endif
    }
    public void subscribeTopic(string TopicName)
    {
#if !UNITY_WEBGL
        if (ws.State == WebSocketStates.Open)
        {
            Debug.Log("{" + (char)34 + "subscribe" + (char)34 + ":" + (char)34 + TopicName + (char)34 + "}");
            ws.Send("{" + (char)34 + "subscribe" + (char)34 + ":" + (char)34 + TopicName + (char)34 + "}");
            PhotonForSpellingQuiz.instance.JoinPanel.SetActive(false);
            PhotonForSpellingQuiz.instance.lobbyPanel.SetActive(true);
            if (!PhotonForSpellingQuiz.instance.debugText.activeSelf)
            {
                PhotonForSpellingQuiz.instance.debugText.SetActive(true);
            }
            //chatPanel.SetActive(true);
            PhotonForSpellingQuiz.instance.isSubscribed = true;
            PhotonForSpellingQuiz.instance.SendMessageToAll(PhotonForSpellingQuiz.instance.userID + " Joined!!!");
            //PhotonForSpellingQuiz.instance.debugText.GetComponent<TextMeshProUGUI>().text = PhotonForSpellingQuiz.instance.debugText.GetComponent<TextMeshProUGUI>().text + "\n" + "ass";
            //ProductionSendValue._instance.SendMyMsgToWarehouse("SendBOT", "S", OrderManager._instance.objRequest._baseLine + ",drone", "S", "vWarehouse");
        }
        else
        {
            throw new Exception("Group Name can't be subscribed when websocket connection state not in open state.");
        }
#endif
    }

    public void send_status(string my_param, string typetosend, string my_paramV, string my_paramD,
                            string device_Name, string macAddress, string deviceType, string groupId)
    {
        string mystring = "{" + (char)34 + "msgType" + (char)34 + ":" + (char)34 + "UPDATE_" + typetosend + (char)34 + "," +
                              (char)34 + "macAddress" + (char)34 + ":" + (char)34 + macAddress + (char)34 + "," +
                              (char)34 + "deviceName" + (char)34 + ":" + (char)34 + device_Name + (char)34 + "," +
                              (char)34 + "deviceType" + (char)34 + ":" + (char)34 + deviceType + (char)34 + "," +
                              (char)34 + "groupId" + (char)34 + ":" + (char)34 + groupId + "_in" + (char)34 + "," +
                              (char)34 + "param" + (char)34 + ":" + (char)34 + my_param + (char)34 + "," +
                              (char)34 + "value" + (char)34 + ":" + (char)34 + my_paramV + (char)34 + "," +
                              (char)34 + "valueDimension" + (char)34 + ":" + (char)34 + my_paramD + (char)34 + "," +
                              (char)34 + "topic" + (char)34 + ":" + (char)34 + "in/" + groupId + "_in/vMANUFACTURING" + (char)34 + "}";
#if UNITY_WEBGL
        Calls.CST_WebGL_SendWebSocketMessage(mystring);
#else
        if (ws.State == WebSocketStates.Open)
        {
            ws.Send(mystring);
        }
        else
        {
            throw new Exception("Status can't be sent when websocket connection state not in open state.");
        }
#endif
    }

    public void send_statusWarehouse(string my_param, string typetosend, string my_paramV, string my_paramD,
                            string device_Name, string macAddress, string deviceType, string groupId, string sendTo)
    {
        string mystring = "{" + (char)34 + "msgType" + (char)34 + ":" + (char)34 + "UPDATE_" + typetosend + (char)34 + "," +
                              (char)34 + "macAddress" + (char)34 + ":" + (char)34 + macAddress + (char)34 + "," +
                              (char)34 + "deviceName" + (char)34 + ":" + (char)34 + device_Name + (char)34 + "," +
                              (char)34 + "deviceType" + (char)34 + ":" + (char)34 + deviceType + (char)34 + "," +
                              (char)34 + "groupId" + (char)34 + ":" + (char)34 + groupId + "_in" + (char)34 + "," +
                              (char)34 + "param" + (char)34 + ":" + (char)34 + my_param + (char)34 + "," +
                              (char)34 + "value" + (char)34 + ":" + (char)34 + my_paramV + (char)34 + "," +
                              (char)34 + "valueDimension" + (char)34 + ":" + (char)34 + my_paramD + (char)34 + "," +
                              (char)34 + "topic" + (char)34 + ":" + (char)34 + "in/" + groupId + "_" + sendTo + "/vRMWIntegration" + (char)34 + "}";

#if UNITY_WEBGL
        Calls.CST_WebGL_SendWebSocketMessage(mystring);
#else
        if (ws.State == WebSocketStates.Open)
        {
            ws.Send(mystring);
        }
        else
        {
            throw new Exception("Status can't be sent when websocket connection state not in open state.");
        }
#endif
    }
}

