using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public virtual async Task OnConnected(string socketID, WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(socketID, socket);

            await SendMessageAsync(socket, new Message()
            {
                MessageType = MessageType.ConnectionEvent,
                Data = WebSocketConnectionManager.GetId(socket)
            }).ConfigureAwait(false);
        }
        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket)).ConfigureAwait(false);
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
            catch (Exception ex)
            {
                // 如果推送失败,清除连接
                await OnDisconnected(socket);
            }
        }
        public async Task SendMessageAsync(string socketId, Message message)
        {
            var result = WebSocketConnectionManager.GetSocketById(socketId);
            if (result == null)
                return;
            await SendMessageAsync(result, message).ConfigureAwait(false);
        }
        public async Task SendMessageToAllAsync(Message message)
        {
            foreach (var pair in WebSocketConnectionManager.GetAll())
            {
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message).ConfigureAwait(false);
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
        public async Task ReceiveAsync(WebSocket socket, string result)
        {
            if (string.IsNullOrWhiteSpace(result))
            {
                await SendMessageAsync(socket, new Message()
                {
                    MessageType = MessageType.Error,
                    Data = $"emtity parameters!"
                }).ConfigureAwait(false);
            }

            try
            {
                await SendMessageAsync(socket, new Message()
                {
                    MessageType = MessageType.Text,
                    Data = result
                }).ConfigureAwait(false);
            }
            catch (TargetParameterCountException)
            {
                await SendMessageAsync(socket, new Message()
                {
                    MessageType = MessageType.Error,
                    Data = $"does not take parameters!"
                }).ConfigureAwait(false);
            }

            catch (ArgumentException)
            {
                await SendMessageAsync(socket, new Message()
                {
                    MessageType = MessageType.Error,
                    Data = $"takes different arguments!"
                }).ConfigureAwait(false);
            }
        }
    }
}
