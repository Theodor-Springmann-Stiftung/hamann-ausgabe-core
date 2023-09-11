var stateSC = null;
var stateValidation = null;
var stateReload = null;
var stateCommit = null;
var firstMessage = true;
var commsLog = document.getElementById("commsLog");
var commsNot = document.getElementById("comm-notifications");
var socket;

var scheme = document.location.protocol === "https:" ? "wss" : "ws";
var port = document.location.port ? (":" + document.location.port) : "";

var connectionUrl = scheme + "://" + document.location.hostname + port + "/WS" ;

function htmlEscape(str) {
    return str.toString()
        .replace(/&/g, '&amp;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');
}


socket = new WebSocket(connectionUrl);
socket.onopen = function (event) {
    socket.send("Hello");
    updateMessage();
};
socket.onclose = function (event) {
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
            setTimeout(() =>  {
                commsNot.remove();
                socket.close(1000, "bye");
                location.reload();
            }, 1500);
            
        }
    } else if (msg.SC != null) {
        stateSC = msg.SC;
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
                if (stateValidation == 0 ) {
                    commsLog.innerHTML = 'Der angezeigte Stand ist nicht aktuell. ' +
                        '<a href="/Admin">Fehler beheben</a>';
                    if (!firstMessage) commsNot.classList.add("imp");
                } else if (stateValidation == 1) {
                    commsLog.innerHTML = "Der Server arbeitet...";
                } else {
                    if (stateCommit != null) {
                        commsLog.innerHTML = "commit " + 
                            stateCommit.Commit.substring(0, 7) +
                            " geladen"
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

        // closeButton.onclick = function () {
        //     if (!socket || socket.readyState !== WebSocket.OPEN) {
        //         alert("socket not connected");
        //     }
        //     socket.close(1000, "Closing from client");
        // };

        // sendButton.onclick = function () {
        //     if (!socket || socket.readyState !== WebSocket.OPEN) {
        //         alert("socket not connected");
        //     }
        //     var data = sendMessage.value;
        //     socket.send(data);
        //     commsLog.innerHTML += '<tr>' +
        //         '<td class="commslog-client">Client</td>' +
        //         '<td class="commslog-server">Server</td>' +
        //         '<td class="commslog-data">' + htmlEscape(data) + '</td></tr>';
        // };

