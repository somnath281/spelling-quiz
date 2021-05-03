var SAP_CST_WebGL_ShowcaseJSLib = {

    CST_WebGL_Initiate: function (webGLLibariesLoadedCallback) {

        webGLLibariesLoadedCallback = Pointer_stringify(webGLLibariesLoadedCallback);

        // Global object
        window.SAP_CST_WebGL_Showcase = {};

        // Default Configuration
        var getDefaultWebsocketConfiguration = function () {
            return {
                URL: "wss://mgwws.hana.ondemand.com/endpoints/v0/ws",
                SubscribeTopics: [],
                onOpenEvent: null,
                onMessageEvent: null,
                onErrorEvent: null,
                onCloseEvent: null
            };
        };

        // WebSocket Connection
        var webSocketConnecton = {
            constructor: function (configuration) {
                // Assigns configuration
                this.configuration = configuration;

                // Creates WebSocket instance
                this.websocket = new sap.ui.core.ws.WebSocket(this.configuration.URL);
                this.websocket.attachOpen(this.onWebsocketOpen.bind(this));
                this.websocket.attachMessage(this.onWebsocketMessage.bind(this));
                this.websocket.attachError(this.onWebsocketError.bind(this));
                this.websocket.attachClose(this.onWebsocketClose.bind(this));
            },

            onWebsocketOpen: function (oEvent) {
                console.log("Websocket opened - " + new Date());

                var i = 0;
                for (; i < this.configuration.SubscribeTopics.length; i++) {
                    var strSubscribeMessage = '{ "subscribe":"' + this.configuration.SubscribeTopics[i] + '"  }';
                    this.websocket.send(strSubscribeMessage);
                    console.log(strSubscribeMessage);
                }

                if (this.configuration.onOpenEvent !== null) {
                    SendMessage(this.configuration.onOpenEvent.GameObjectName, this.configuration.onOpenEvent.MethodName);
                }
            },

            onWebsocketMessage: function (event) {
                var message = event.getParameters().data;
                var msg = JSON.parse(message);

                if (msg._MessageGateway_MessageType !== undefined) {
                    if (msg._MessageGateway_MessageType === "WebSocket Keep Alive Message") {
                        return;
                    }
                }

                if (this.configuration.onMessageEvent !== null) {
                    SendMessage(this.configuration.onMessageEvent.GameObjectName, this.configuration.onMessageEvent.MethodName, message);
                }
            },

            onWebsocketError: function (event) {
                console.log("Websocket error - " + new Date());
                console.log(event);

                if (this.configuration.onErrorEvent !== null) {
                    SendMessage(this.configuration.onErrorEvent.GameObjectName, this.configuration.onErrorEvent.MethodName);
                }
            },

            onWebsocketClose: function (event) {
                console.log("Websocket closed - " + new Date());
                console.log(event);

                if (this.configuration.onCloseEvent !== null) {
                    SendMessage(this.configuration.onCloseEvent.GameObjectName, this.configuration.onCloseEvent.MethodName);
                }
            }
        }

        // Ajax Calls
        var ajaxRequest = function (ajaxRequestModel) {
            if (typeof ajaxRequestModel.URL !== "string" && typeof ajaxRequest.Method !== "string") {
                return;
            }

            if (ajaxRequestModel.Method !== "GET" && ajaxRequestModel.Method !== "POST") {
                return;
            }

            var successCallback = function (data, textStatus, jqXHR) {
                if (ajaxRequestModel.SuccessCallback !== undefined &&
                    ajaxRequestModel.SuccessCallback !== null) {
                    if (typeof ajaxRequestModel.SuccessCallback.GameObjectName === "string" &&
                        typeof ajaxRequestModel.SuccessCallback.MethodName === "string") {
                        SendMessage(ajaxRequestModel.SuccessCallback.GameObjectName, ajaxRequestModel.SuccessCallback.MethodName);
                    }
                }
            }

            var errorCallback = function (jqXHR, textStatus, errorThrown) {
                if (ajaxRequestModel.ErrorCallback !== undefined &&
                    ajaxRequestModel.ErrorCallback !== null) {
                    if (typeof ajaxRequestModel.ErrorCallback.GameObjectName === "string" &&
                        typeof ajaxRequestModel.ErrorCallback.MethodName === "string") {
                        SendMessage(ajaxRequestModel.ErrorCallback.GameObjectName, ajaxRequestModel.ErrorCallback.MethodName);
                    }
                }
            }

            var async = true;

            if (typeof ajaxRequestModel.Async === "boolean") {
                if (ajaxRequestModel.Async === false) {
                    async = false;
                }
            }

            var request = {};

            request.url = ajaxRequestModel.URL;
            request.async = async;
            request.type = ajaxRequestModel.Method;
            request.success = successCallback;
            request.error = errorCallback;

            if (ajaxRequestModel.Method === "POST") {
                request.data = ajaxRequestModel.JSONString;
                request.contentType = "application/json; charset=utf-8";
            }

            // Replaces with destination URL
            var iIndex = 0;
            for (; iIndex < window.SAP_CST_WebGL_Showcase.DestinationURLs.length; iIndex++) {
                var objUrl = window.SAP_CST_WebGL_Showcase.DestinationURLs[iIndex];

                // Checks URL starts with Original URL
                if (request.url.toUpperCase().startsWith(objUrl.OriginalURL.toUpperCase())) {
                    request.url = request.url.substring(objUrl.OriginalURL.length, request.url.length)

                    console.log(request.url);
                    request.url = objUrl.DestinationURL + request.url;
                    console.log(request.url);
                }
            }

            var jqXHR = $.ajax(request);

            if (async) return;

            return jqXHR.responseText;
        }

        // Return string
        var returnString = function (value) {
            if (typeof value !== "string") return null;

            //Get size of the string
            var bufferSize = lengthBytesUTF8(value) + 1;
            //Allocate memory space
            var buffer = _malloc(bufferSize);
            //Copy old data to the new one then return it
            stringToUTF8(value, buffer, bufferSize);
            return buffer;
        }

        // Update SAP_CST_WebGL_Showcase object
        window.SAP_CST_WebGL_Showcase = {
            WebSocket: {
                GetDefaultConfiguration: getDefaultWebsocketConfiguration,
                CustomConfiguration: null,
                LibrariesLoaded: false,
                Connection: null,
                Instance: null
            },
            AjaxRequest: ajaxRequest,
            ReturnString: returnString,
            DestinationURLs: []
        };

        // WebSocket Connection
        var onSAPUICoreLibraryLoaded = function () {
            console.log("SAP UI Core Loaded");

            // Loads libraries
            jQuery.sap.require("sap.ui.base.Object");
            jQuery.sap.require("sap.ui.core.ws.WebSocket");

            // Assing webSocketConnection class to global variable
            window.SAP_CST_WebGL_Showcase.WebSocket.Connection = sap.ui.base.Object.extend("SAP.CST.WebGLShowcase.WebSocket.Connection", webSocketConnecton);

            // Adds connection
            window.SAP_CST_WebGL_Showcase.WebSocket.LibrariesLoaded = true;
            if (typeof webGLLibariesLoadedCallback === "string") {
                var objWebGLLibariesLoadedCallback = JSON.parse(webGLLibariesLoadedCallback);
                SendMessage(objWebGLLibariesLoadedCallback.GameObjectName, objWebGLLibariesLoadedCallback.MethodName);
            }

            // Loads URls
            $.ajax({
                url: "/neo-app.json",
                success: function (data, textStatus, jqXHR) {
                    var iIndex = 0;
                    window.SAP_CST_WebGL_Showcase = { DestinationURLs: [] };
                    for (; iIndex < data.routes.length; iIndex++) {
                        var route = data.routes[iIndex];

                        if (route.originalURL === undefined || route.originalURL === null) {
                            continue;
                        }

                        if (route.target.type === "destination") {
                            window.SAP_CST_WebGL_Showcase.DestinationURLs.push({
                                DestinationURL: route.path,
                                OriginalURL: route.originalURL
                            });
                        }
                    }
                }
            });
        };

        // Adds SAP UI Core library to body element
        var sapUICoreScriptElement = document.createElement("script");
        sapUICoreScriptElement.setAttribute("type", "text/javascript");
        sapUICoreScriptElement.setAttribute("src", "https://sapui5.hana.ondemand.com/resources/sap-ui-core.js");
        sapUICoreScriptElement.addEventListener("load", onSAPUICoreLibraryLoaded);
        document.head.appendChild(sapUICoreScriptElement);
    },

    CST_WebGL_CustomWebSocketConfig_SubscribeTopics: function (subscribeTopics) {
        // Gets JSON scring value of subscribeTopics
        subscribeTopics = Pointer_stringify(subscribeTopics);

        if (window.SAP_CST_WebGL_Showcase.WebSocket.CustomConfiguration === null) {
            window.SAP_CST_WebGL_Showcase.WebSocket.CustomConfiguration = window.SAP_CST_WebGL_Showcase.WebSocket.GetDefaultConfiguration();
        }

        window.SAP_CST_WebGL_Showcase.WebSocket.CustomConfiguration.SubscribeTopics = JSON.parse(subscribeTopics);
    },

    CST_WebGL_CustomWebSocketConfig_WebSocketEvents: function (webSocketEvents) {
        // Gets JSON string value of webSocketEvents
        webSocketEvents = Pointer_stringify(webSocketEvents);
        webSocketEvents = JSON.parse(webSocketEvents);

        if (window.SAP_CST_WebGL_Showcase.WebSocket.CustomConfiguration === null) {
            window.SAP_CST_WebGL_Showcase.WebSocket.CustomConfiguration = window.SAP_CST_WebGL_Showcase.WebSocket.GetDefaultConfiguration();
        }

        if (webSocketEvents.onWebSocketOpen !== null && webSocketEvents.onWebSocketOpen !== undefined) {
            if (typeof webSocketEvents.onWebSocketOpen.GameObjectName === "string" &&
                typeof webSocketEvents.onWebSocketOpen.MethodName === "string") {
                if (webSocketEvents.onWebSocketOpen.GameObjectName !== "" && webSocketEvents.onWebSocketOpen.MethodName !== "") {
                    window.SAP_CST_WebGL_Showcase.WebSocket.CustomConfiguration.onOpenEvent = webSocketEvents.onWebSocketOpen;
                }
            }
        }

        if (webSocketEvents.onWebSocketError !== null && webSocketEvents.onWebSocketError !== undefined) {
            if (typeof webSocketEvents.onWebSocketError.GameObjectName === "string" &&
                typeof webSocketEvents.onWebSocketError.MethodName === "string") {
                if (webSocketEvents.onWebSocketError.GameObjectName !== "" && webSocketEvents.onWebSocketError.MethodName !== "") {
                    window.SAP_CST_WebGL_Showcase.WebSocket.CustomConfiguration.onErrorEvent = webSocketEvents.onWebSocketError;
                }
            }
        }

        if (webSocketEvents.onWebSocketClose !== null && webSocketEvents.onWebSocketClose !== undefined) {
            if (typeof webSocketEvents.onWebSocketClose.GameObjectName === "string" &&
                typeof webSocketEvents.onWebSocketClose.MethodName === "string") {
                if (webSocketEvents.onWebSocketClose.GameObjectName !== "" && webSocketEvents.onWebSocketClose.MethodName !== "") {
                    window.SAP_CST_WebGL_Showcase.WebSocket.CustomConfiguration.onCloseEvent = webSocketEvents.onWebSocketClose;
                }
            }
        }

        if (webSocketEvents.onWebSocketMessage !== null && webSocketEvents.onWebSocketMessage !== undefined) {
            if (typeof webSocketEvents.onWebSocketMessage.GameObjectName === "string" &&
                typeof webSocketEvents.onWebSocketMessage.MethodName === "string") {
                if (webSocketEvents.onWebSocketMessage.GameObjectName !== "" && webSocketEvents.onWebSocketMessage.MethodName !== "") {
                    window.SAP_CST_WebGL_Showcase.WebSocket.CustomConfiguration.onMessageEvent = webSocketEvents.onWebSocketMessage;
                }
            }
        }
    },

    CST_WebGL_OpenWebSocketConnection: function () {
        // Throws error message if WebSocket libraries are not loaded
        if (window.SAP_CST_WebGL_Showcase.WebSocket.LibrariesLoaded === false) throw "WebSocket libraries not loaded";

        // WebSocket Configuration
        var webSocketConfiguration = null;

        if (window.SAP_CST_WebGL_Showcase.WebSocket.CustomConfiguration === null) {
            webSocketConfiguration = window.SAP_CST_WebGL_Showcase.WebSocket.GetDefaultConfiguration();
        }
        else {
            webSocketConfiguration = JSON.parse(JSON.stringify(window.SAP_CST_WebGL_Showcase.WebSocket.CustomConfiguration));
        }

        window.SAP_CST_WebGL_Showcase.WebSocket.Instance = new window.SAP_CST_WebGL_Showcase.WebSocket.Connection(webSocketConfiguration);
    },

    CST_WebGL_GetWebSocketConnectionReadyState: function () {
        //Closed - 3
        //Closing - 2
        //Connecting - 0
        //Open - 1

        if (window.SAP_CST_WebGL_Showcase.WebSocket.Instance != null) {
            return window.SAP_CST_WebGL_Showcase.WebSocket.Instance.websocket.getReadyState();
        }
        else {
            return -1;
        }
    },

    CST_WebGL_SendWebSocketMessage: function (message) {
        message = Pointer_stringify(message);
        if (window.SAP_CST_WebGL_Showcase.WebSocket.Instance !== null) {
            if (window.SAP_CST_WebGL_Showcase.WebSocket.Instance.websocket.getReadyState() === sap.ui.core.ws.ReadyState.OPEN) {
                window.SAP_CST_WebGL_Showcase.WebSocket.Instance.websocket.send(message);
            }
        }
    },

    CST_WebGL_AjaxRequest: function (ajaxRequestModel) {
        if (typeof ajaxRequestModel !== "number") {
            return "";
        }

        ajaxRequestModel = Pointer_stringify(ajaxRequestModel);

        if (typeof ajaxRequestModel !== "string") {
            return "";
        }

        ajaxRequestModel = JSON.parse(ajaxRequestModel);

        if (typeof ajaxRequestModel !== "object") {
            return "";
        }

        return window.SAP_CST_WebGL_Showcase.ReturnString(window.SAP_CST_WebGL_Showcase.AjaxRequest(ajaxRequestModel));
    },

    CST_WebGL_GetURLValueFromParam: function (key) {
        key = Pointer_stringify(key);

        var group = (new URLSearchParams(window.location.search)).get(key);

        if (group === null) return null;

        group = group.replace(/ /g, "_");

        //Get size of the string
        var bufferSize = lengthBytesUTF8(group) + 1;
        //Allocate memory space
        var buffer = _malloc(bufferSize);
        //Copy old data to the new one then return it
        stringToUTF8(group, buffer, bufferSize);
        return buffer;
    }
}

mergeInto(LibraryManager.library, SAP_CST_WebGL_ShowcaseJSLib);