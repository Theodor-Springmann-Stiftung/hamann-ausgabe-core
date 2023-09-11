using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using HaWeb;
using HaWeb.FileHelpers;
using HaWeb.Models;
using HaWeb.XMLParser;
using Microsoft.FeatureManagement;

public class WebSocketMiddleware : IMiddleware {
    internal enum ValidationState {
        False,
        Parsing,
        True
    }

    internal class FileState {
        public ValidationState ValidationState { get; private set; }

        public FileState(ValidationState state) {
            this.ValidationState = state;
        }

        public FileState(XMLParsingState? state) {
            if (state == null) ValidationState = ValidationState.Parsing;
            else if (state.ValidState == true) ValidationState = ValidationState.True;
            else ValidationState = ValidationState.False;
        }
    }

    private readonly IFeatureManager _featureManager;
    private readonly IXMLInteractionService _xmlService;
    private readonly IXMLFileProvider _xmlProvider;

    private List<WebSocket>? _openSockets;

    public WebSocketMiddleware(IXMLFileProvider xmlprovider, IXMLInteractionService xmlservice, IFeatureManager featuremanager){
        this._xmlProvider = xmlprovider;
        this._xmlService = xmlservice;
        this._featureManager = featuremanager;
        if (_openSockets == null) _openSockets = new List<WebSocket>();
        _Subscribe();
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate requestDelegate) {
        if (!context.WebSockets.IsWebSocketRequest || context.Request.Path != "/WS") {
            // this case works perfectly fine for regular REST, middleware gets called.
            await requestDelegate.Invoke(context);
            return;
        }
        
        if (await _featureManager.IsEnabledAsync(Features.Notifications)) {
            using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync()) {
                await HandleConnection(context, webSocket);
            }
        }
    }

    private void _Subscribe() {
        _xmlProvider.FileChange += _HandleFileChange;
        _xmlProvider.NewState += _HandleNewState;
        _xmlProvider.NewData += _HandleNewData;
        _xmlProvider.ConfigReload += _HandleConfigReload;
        _xmlService.SyntaxCheck += _HandleSyntaxCheck;
    }

    private async Task HandleConnection(HttpContext context, WebSocket webSocket) {
        var buffer = new byte[1024 * 4];
        _openSockets!.Add(webSocket);
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        while (!result.CloseStatus.HasValue) {
            var state = _xmlProvider.GetGitState();
            await webSocket.SendAsync(_SerializeToBytes(state), WebSocketMessageType.Text, true, CancellationToken.None);
            await webSocket.SendAsync(_SerializeToBytes(new FileState(_xmlService.GetState())), result.MessageType, true, CancellationToken.None);
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        _openSockets!.Remove(webSocket);
    }

    private async void _HandleFileChange(object? sender, GitState? state) {
        await _SendToAll(state);
        await _SendToAll(new FileState(ValidationState.Parsing));
    }

    private async void _HandleNewState(object? sender, XMLParsingState? state) {
        if (state == null || !state.ValidState)
            await _SendToAll(new FileState(state));
    }

    private async void  _HandleNewData(object? sender, EventArgs _) {
        await _SendToAll(new { reload = true });
    }

    private async void _HandleConfigReload(object? sender, EventArgs _) {
        await _SendToAll(new { configreload = true });
    }

    private async void _HandleSyntaxCheck(object? sender, Dictionary<string, SyntaxCheckModel>? state) {
        if (state != null && state.Any()) {
            foreach (var c in state) 
                if (c.Value.Errors != null) {
                    await _SendToAll(new { SC = false });
                    return;
                }
            await _SendToAll(new { SC = true });
        }
        await _SendToAll(new { SC = (String?)null });
    }

    private async Task _SendToAll<T>(T msg) {
        if (_openSockets == null) return;
        foreach (var socket in _openSockets) {
            await socket.SendAsync(_SerializeToBytes(msg), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    private ArraySegment<byte> _SerializeToBytes<T>(T o) {
        var json = JsonSerializer.Serialize<T>(o);
        if (String.IsNullOrWhiteSpace(json)) {
            return new ArraySegment<byte>(new byte[] { }, 0, 0);
        }
        var bytes = Encoding.UTF8.GetBytes(json);
        return new ArraySegment<byte>(bytes, 0, bytes.Length);
    }
}