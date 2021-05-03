using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectToWebsocket : MonoBehaviour
{
    public static ConnectToWebsocket _instance;
    WebSocketToHCP webSocketToHCP;
    int numTries = 0;
    bool blSend = false;
#if UNITY_WEBGL
    bool WebGLLibrariesLoaded = false;
#endif
    void Awake()
    {
        _instance = this;

    }
    // Start is called before the first frame update
    void Start()
    {
        string host = "wss://mgwws.hana.ondemand.com/endpoints/v1/ws";
        int connectionTimeout = 5;
        string proxy = string.Empty;
        string subscribeTopic = "somChannel";
        blSend = true;
#if UNITY_WEBGL
        // Initiate WebGL
        string strWebGLLibrariesLoadedCallback = JsonUtility.ToJson(new Callback
        {
            GameObjectName = "GameManager",
            MethodName = "WebGLLibariesLoadedCallback"
        });
        print(strWebGLLibrariesLoadedCallback);
        Calls.CST_WebGL_Initiate(strWebGLLibrariesLoadedCallback);

        // Websocket message received event
        WebSocketEvents webSocketEvents = new WebSocketEvents();
        webSocketEvents.onWebSocketMessage = new Callback()
        {
            GameObjectName = "GameManager",
            MethodName = "onMessageReceived"
        };
        Calls.CST_WebGL_CustomWebSocketConfig_WebSocketEvents(JsonUtility.ToJson(webSocketEvents));
        webSocketToHCP = new WebSocketToHCP(host, connectionTimeout, proxy, subscribeTopic);
        StartCoroutine(Connect_To_HCP());
        numTries = 0;
#endif
    }
#if UNITY_WEBGL

    public void WebGLLibariesLoadedCallback()
    {
        WebGLLibrariesLoaded = true;
    }

#endif
    IEnumerator Connect_To_HCP()
    {
        while (true)
        {
            ConnectToHCP();
            yield return new WaitForSeconds(1f);
        }
    }
    void ConnectToHCP()
    {
        //Used for connecting to HCP

        if (blSend == false)
        {
            return;
        }

        if (webSocketToHCP.ConnectionState == WebSocket_Connection_State.Open)
        {
#if !UNITY_WEBGL

#endif
            numTries = 0;
        }
        else if ((webSocketToHCP.ConnectionState == WebSocket_Connection_State.Ready ||
            webSocketToHCP.ConnectionState == WebSocket_Connection_State.Closed) &&
            numTries < 5)
        {
            //statusButton.GetComponentInChildren<Text>().text = "Connecting to HCP";
            Debug.Log("Connecting to HCP");

#if !UNITY_WEBGL

#else
            if (WebGLLibrariesLoaded)
            {
                webSocketToHCP.ConnectToHCP();
                numTries++;
            }
#endif
        }
        
    }
    void onMessageReceived(string message)
    {
        Debug.Log("Message Received");
        Debug.Log(message);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
