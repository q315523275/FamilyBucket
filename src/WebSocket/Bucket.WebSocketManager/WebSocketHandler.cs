
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.WebSocketManager
{
    public abstract class WebSocketHandler
    {
        protected WebSocketConnectionManager WebSocketConnectionManager { get; set; }
        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }
        public virtual async Task OnConnected(WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(socket);

            await SendMessageAsync(socket, new Message()
            {
                MessageType = MessageType.ConnectionEvent,
                Data = WebSocketConnectionManager.GetId(socket)
            }).ConfigureAwait(false);
        }
        public virtual async Task OnConnected(string socketId, WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(socketId, socket);

            await SendMessageAsync(socket, new Message()
            {
                MessageType = MessageType.ConnectionEvent,
                Data = WebSocketConnectionManager.GetId(socket)
            }).ConfigureAwait(false);
        }
        public virtual async Task OnDisconnected(WebSocket socket)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            if (!string.IsNullOrWhiteSpace(socketId))
                await WebSocketConnectionManager.RemoveSocket(socketId).ConfigureAwait(false);
        }
        public async Task SendMessageAsync(WebSocket socket, Message message)
        {
            if (socket.State != WebSocketState.Open)
                return;
            var serializedMessage = JsonConvert.SerializeObject(message);
            var encodedMessage = Encoding.UTF8.GetBytes(serializedMessage);
            try
            {
                await socket.SendAsync(buffer: new ArraySegment<byte>(array: encodedMessage,
                                                                      offset: 0,
                                                                      count: encodedMessage.Length),
                                       messageType: WebSocketMessageType.Text,
                                       endOfMessage: true,
                                       cancellationToken: CancellationToken.None).ConfigureAwait(false);
            }
            catch (WebSocketException e)
            {
                if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                {
                    await OnDisconnected(socket);
                }
            }
        }
        public async Task SendMessageAsync(string socketId, Message message)
        {
            var socket = WebSocketConnectionManager.GetSocketById(socketId);
            if (socket != null)
                await SendMessageAsync(socket, message).ConfigureAwait(false);
        }
        public async Task SendMessageToAllAsync(Message message)
        {
            foreach (var pair in WebSocketConnectionManager.GetAll())
            {
                try
                {
                    if (pair.Value.State == WebSocketState.Open)
                        await SendMessageAsync(pair.Value, message).ConfigureAwait(false);
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        await OnDisconnected(pair.Value);
                    }
                }
            }
        }
        public async Task SendMessageToGroupAsync(string groupID, Message message)
        {
            var sockets = WebSocketConnectionManager.GetAllFromGroup(groupID);
            if (sockets != null)
            {
                foreach (var socket in sockets)
                {
                    await SendMessageAsync(socket, message);
                }
            }
        }
        public async Task SendMessageToGroupAsync(string groupID, Message message, string except)
        {
            var sockets = WebSocketConnectionManager.GetAllFromGroup(groupID);
            if (sockets != null)
            {
                foreach (var id in sockets)
                {
                    if (id != except)
                        await SendMessageAsync(id, message);
                }
            }
        }
        public virtual async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, Message receivedMessage)
        {
            try
            {
                await SendMessageAsync(socket, receivedMessage).ConfigureAwait(false);
            }
            catch (TargetParameterCountException)
            {
                await SendMessageAsync(socket, new Message() { MessageType = MessageType.Error, Data = $"does not take parameters!" }).ConfigureAwait(false);
            }

            catch (ArgumentException)
            {
                await SendMessageAsync(socket, new Message() { MessageType = MessageType.Error, Data = $"takes different arguments!" }).ConfigureAwait(false);
            }
        }
    }
}
