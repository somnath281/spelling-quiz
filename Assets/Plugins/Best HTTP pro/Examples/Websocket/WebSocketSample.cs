#if !BESTHTTP_DISABLE_WEBSOCKET

using System;
using System.IO;
using System.Net;
using UnityEngine;

namespace BestHTTP.Examples
{
    public class WebSocketSample : MonoBehaviour
    {
        #region Private Fields

        /// <summary>
        /// The WebSocket address to connect
        /// </summary>
        //string address = "wss://echo.websocket.org";
        string address = "wss://mgwws.hana.ondemand.com/endpoints/v1/ws";

        /// <summary>
        /// Default text to send
        /// </summary>
        string msgToSend = "som zen";

        /// <summary>
        /// Debug text to draw on the gui
        /// </summary>
        string Text = string.Empty;

        /// <summary>
        /// Saved WebSocket instance
        /// </summary>
        WebSocket.WebSocket webSocket;

        /// <summary>
        /// GUI scroll position
        /// </summary>
        Vector2 scrollPos;

        #endregion

        #region Unity Events
        private void Start()
        {
            getProductionOrderCount1();
        }
        void OnDestroy()
        {
            if (webSocket != null)
            {
                webSocket.Close();
            }
        }

        void OnGUI()
        {
            GUIHelper.DrawArea(GUIHelper.ClientArea, true, () =>
                {
                    scrollPos = GUILayout.BeginScrollView(scrollPos);
                    GUILayout.Label(Text);
                    GUILayout.EndScrollView();

                    GUILayout.Space(5);

                    GUILayout.FlexibleSpace();

                    address = GUILayout.TextField(address);

                    if (webSocket == null && GUILayout.Button("Open Web Socket"))
                    {
                        // Create the WebSocket instance
                        webSocket = new WebSocket.WebSocket(new Uri(address));

#if !UNITY_WEBGL
                        webSocket.StartPingThread = true;

#if !BESTHTTP_DISABLE_PROXY
                        if (HTTPManager.Proxy != null)
                            webSocket.InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
#endif
#endif

                        // Subscribe to the WS events
                        webSocket.OnOpen += OnOpen;
                        webSocket.OnMessage += OnMessageReceived;
                        webSocket.OnClosed += OnClosed;
                        webSocket.OnError += OnError;

                        // Start connecting to the server
                        webSocket.Open();

                        Text += "Opening Web Socket...\n";
                    }

                    if (webSocket != null && webSocket.IsOpen)
                    {
                        GUILayout.Space(10);

                        GUILayout.BeginHorizontal();
                        msgToSend = GUILayout.TextField(msgToSend);

                        GUILayout.EndHorizontal();

                        if (GUILayout.Button("Send", GUILayout.MaxWidth(70)))
                        {
                            Text += "Sending message...\n";

                            // Send message to the server
                            //webSocket.Send(msgToSend);
                            send_status("sendParam", "S", msgToSend, "S", "S", "20:20:1:1:1", "S", "som");
                            
                        }

                        GUILayout.Space(10);

                        if (GUILayout.Button("Close"))
                        {
                            // Close the connection
                            webSocket.Close(1000, "Bye!");
                        }
                    }
                });
        }

        #endregion
        public void getProductionOrderCount1()
        {
            print("getProductionOrderCount1");
            HTTPRequest request = new HTTPRequest(new Uri("https://ev3pump3a9820668f.hana.ondemand.com/vManufacturing/services/anonymous/Get_PO_Count.xsjs?prodline=iot_manufacturing_dev"), OnRequestFinished);
            request.Send();
        }
        void OnRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            Debug.Log("Request Finished! Text received: " + response.DataAsText);
        }

        public int getProductionOrderCount()
        {
            int OrderCount = 0;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => { return true; };
            string url = "https://ev3pump3a9820668f.hana.ondemand.com/vManufacturing/services/anonymous/Get_PO_Count.xsjs?prodline=iot_manufacturing_dev";
            //Integration Mode
            //string url = "https://testc5245131trial.hanatrial.ondemand.com/vManufacturing/services/anonymous/Get_PO_Count.xsjs?prodline=" + _prodline;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            try
            {

                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                // Set credentials to use for this request.
                httpWebRequest.Credentials = CredentialCache.DefaultCredentials;

                //if (_proxy != string.Empty)
                //{
                //    WebProxy proxy = new WebProxy("http://proxy:8080/", false);
                //    httpWebRequest.Proxy = proxy;
                //}

                var httpRespone = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpRespone.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    OrderCount = int.Parse(result);
                    print(OrderCount);
                    streamReader.Close();
                }
                httpRespone.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                //httpWebRequest.Abort();
            }
            return OrderCount;
        }
        #region WebSocket Event Handlers

        /// <summary>
        /// Called when the web socket is open, and we are ready to send and receive data
        /// </summary>
        void OnOpen(WebSocket.WebSocket ws)
        {
            Text += string.Format("-WebSocket Open!\n");

            Debug.Log("{" + (char)34 + "subscribe" + (char)34 + ":" + (char)34 + "in/som/unity" + (char)34 + "}");
            ws.Send("{" + (char)34 + "subscribe" + (char)34 + ":" + (char)34 + "in/som/unity" + (char)34 + "}");

        }
        public void send_status(string my_param, string typetosend, string my_paramV, string my_paramD,
                                    string device_Name, string macAddress, string deviceType, string groupId)
        {

            string mystring = "{" + (char)34 + "msgType" + (char)34 + ":" + (char)34 + "UPDATE_" + typetosend + (char)34 + "," +
                              (char)34 + "macAddress" + (char)34 + ":" + (char)34 + macAddress + (char)34 + "," +
                              (char)34 + "deviceName" + (char)34 + ":" + (char)34 + device_Name + (char)34 + "," +
                              (char)34 + "deviceType" + (char)34 + ":" + (char)34 + deviceType + (char)34 + "," +
                              (char)34 + "groupId" + (char)34 + ":" + (char)34 + groupId + (char)34 + "," +
                              (char)34 + "param" + (char)34 + ":" + (char)34 + my_param + (char)34 + "," +
                              (char)34 + "value" + (char)34 + ":" + (char)34 + my_paramV + (char)34 + "," +
                              (char)34 + "valueDimension" + (char)34 + ":" + (char)34 + my_paramD + (char)34 + "," +
                              (char)34 + "topic" + (char)34 + ":" + (char)34 + "in/som/unity" + (char)34 + "}";

            webSocket.Send(mystring);


        }
        /// <summary>
        /// Called when we received a text message from the server
        /// </summary>
        void OnMessageReceived(WebSocket.WebSocket ws, string message)
        {
            Text += string.Format("-Message received: {0}\n", message);
        }

        /// <summary>
        /// Called when the web socket closed
        /// </summary>
        void OnClosed(WebSocket.WebSocket ws, UInt16 code, string message)
        {
            Text += string.Format("-WebSocket closed! Code: {0} Message: {1}\n", code, message);
            webSocket = null;
        }

        /// <summary>
        /// Called when an error occured on client side
        /// </summary>
        void OnError(WebSocket.WebSocket ws, Exception ex)
        {
            string errorMsg = string.Empty;
#if !UNITY_WEBGL || UNITY_EDITOR
            if (ws.InternalRequest.Response != null)
            {
                errorMsg = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);
            }
#endif

            Text += string.Format("-An error occured: {0}\n", (ex != null ? ex.Message : "Unknown Error " + errorMsg));

            webSocket = null;
        }

        #endregion
    }
}

#endif