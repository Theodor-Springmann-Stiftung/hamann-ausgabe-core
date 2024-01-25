// Functions for change notifications and automatic reload via websockets
const startup_websocket = function () {
    function htmlEscape(str) {
        return str.toString()
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
    }


    var stateSC = null;
    var stateValidation = null;
    var stateReload = null;
    var stateCommit = null;
    var statePing = false;
    var firstMessage = true;
    var commsLog = document.getElementById("commsLog");
    var commsNot = document.getElementById("comm-notifications");
    var socket;

    var scheme = document.location.protocol === "https:" ? "wss" : "ws";
    var port = document.location.port ? (":" + document.location.port) : "";
    var wsPingInterval;
    var connectionUrl = scheme + "://" + document.location.hostname + port + "/WS";

    socket = new WebSocket(connectionUrl);
    socket.onopen = function (event) {
        socket.send("Hello");
        wsPingInterval = setInterval(() => {
            socket.send("Ping");
        }, 30000);
        updateMessage();
    };
    socket.onclose = function (event) {
        clearInterval(wsPingInterval);
        updateMessage();
    };
    socket.onerror = updateMessage;
    socket.onmessage = function (event) {
        var msg = JSON.parse(event.data);
        if (msg.ValidationState != null) {
            stateValidation = msg.ValidationState;
            console.log(msg.ValidationState);
            switch (msg.ValidationState) {
                case 0:
                    commsNot.classList.remove("loading");
                    commsNot.classList.remove("green");
                    if (!commsNot.classList.contains("red")) {
                        commsNot.classList.add("red");
                    }
                    updateMessage();
                    break;
                case 1:
                    if (!commsNot.classList.contains("loading")) {
                        commsNot.classList.add("loading");
                    }
                    updateMessage();
                    break;
                case 2:
                    commsNot.classList.remove("red");
                    commsNot.classList.remove("loading");
                    if (!commsNot.classList.contains("green")) {
                        commsNot.classList.add("green");
                    }
                    updateMessage();
                    break;
            }
        } else if (msg.Commit != null) {
            stateCommit = msg;
            updateMessage();
        } else if (msg.reload != null) {
            stateReload = msg.reload;
            if (msg.reload) {
                commsLog.innerHTML = 'Seite wird neu geladen.';
                commsNot.classList.add("imp");
                setTimeout(() => {
                    commsNot.remove();
                    socket.close(1000, "bye");
                    location.reload();
                }, 2000);
            }
        } else if (msg.SC != null) {
            stateSC = msg.SC;
        } else if (msg.Ping != null) {
            statePing = true;
        } else {
            commsLog.innerHTML = htmlEscape(event.data);
        }

    };

    function updateMessage() {
        function disable() {
            commsNot.classList.remove("red");
            commsNot.classList.remove("loading");
            commsNot.classList.remove("green");
        }
        function enable() {

        }

        if (!socket) {
            disable();
        } else {
            switch (socket.readyState) {
                case WebSocket.CLOSED:
                    commsLog.innerHTML = "Keine Verbindung";
                    disable();
                    break;
                case WebSocket.CLOSING:
                    commsLog.innerHTML = "Verbindung wird geschlossen...";
                    disable();
                    break;
                case WebSocket.CONNECTING:
                    commsLog.innerHTML = "Verbinden...";
                    disable();
                    break;
                case WebSocket.OPEN:
                    commsLog.innerHTML = "";
                    // TODO: decide on state what the message is
                    if (stateValidation == 0) {
                        commsLog.innerHTML = 'Der angezeigte Stand ist nicht aktuell. ' +
                            '<a href="/Admin">Fehler beheben</a>';
                        if (!firstMessage) commsNot.classList.add("imp");
                    } else if (stateValidation == 1) {
                        commsLog.innerHTML = "Der Server arbeitet...";
                    } else {
                        if (stateCommit != null) {
                            commsLog.innerHTML = "<a href='https://github.com/Theodor-Springmann-Stiftung/hamann-xml/commit/" + stateCommit.Commit.substring(0, 7) + "'>commit " +
                                stateCommit.Commit.substring(0, 7) +
                                " geladen</a>"
                        } else {
                            commsLog.innerHTML = "OK.";
                        }
                    }
                    firstMessage = false;
                    enable();
                    break;
                default:
                    commsLog.innerHTML = "Unknown WebSocket State: " + htmlEscape(socket.readyState);
                    disable();
                    break;
            }
        }
    }
};

startup_websocket();