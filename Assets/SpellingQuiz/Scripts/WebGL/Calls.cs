using System.Runtime.InteropServices;

public static class Calls
{
    [DllImport("__Internal")]
    public static extern void CST_WebGL_Initiate(string str);


    [DllImport("__Internal")]
    public static extern void CST_WebGL_CustomWebSocketConfig_SubscribeTopics(string subscribeTopics);


    [DllImport("__Internal")]
    public static extern void CST_WebGL_CustomWebSocketConfig_WebSocketEvents(string webSocketEvents);


    [DllImport("__Internal")]
    public static extern int CST_WebGL_GetWebSocketConnectionReadyState();


    [DllImport("__Internal")]
    public static extern void CST_WebGL_OpenWebSocketConnection();


    [DllImport("__Internal")]
    public static extern void CST_WebGL_SendWebSocketMessage(string message);


    [DllImport("__Internal")]
    public static extern string CST_WebGL_AjaxRequest(string ajaxRequestModel);


    [DllImport("__Internal")]
    public static extern string CST_WebGL_GetURLValueFromParam(string key);
}