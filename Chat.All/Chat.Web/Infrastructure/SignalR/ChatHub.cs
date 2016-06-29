﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Infrastructure;
using Microsoft.AspNet.SignalR;

namespace Chat.Web.Infrastructure.SignalR
{
    public class ChatHub : Hub
    {
        private static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        private static List<MessageDetail> CurrentMessage = new List<MessageDetail>();

        public void Connect(string userName)
        {
            string id = Context.ConnectionId;

            //var user = ConnectedUsers.FirstOrDefault(x => x.UserName == userName);

            //if (user == null)
            //{
            //    id = Context.ConnectionId;
            //}
            //else
            //{
            //    id = user.ConnectionId;

            //    // send to caller
            //    Clients.Caller.onConnected(id, userName, ConnectedUsers, CurrentMessage);
            //    return;
            //}

            if (ConnectedUsers.All(x => x.ConnectionId != id))
            {
                ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = userName });

                // send to caller
                Clients.Caller.onConnected(id, userName, ConnectedUsers, CurrentMessage);

                // send to all except caller client
                Clients.AllExcept(id).onNewUserConnected(id, userName);
            }
        }

        public void SendMessageToAll(string userName, string message)
        {
            // store last 100 messages in cache
            AddMessageinCache(userName, message);

            // Broad cast message
            Clients.All.messageReceived(userName, message);
        }

        public void SendPrivateMessage(string toUserId, string message)
        {
            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId) ;
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null && fromUser!=null)
            {
                // send to 
                Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.UserName, message); 

                // send to caller user
                Clients.Caller.sendPrivateMessage(toUserId, fromUser.UserName, message); 
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.UserName);
            }

            return base.OnDisconnected(stopCalled);
        }

        private void AddMessageinCache(string userName, string message)
        {
            CurrentMessage.Add(new MessageDetail { UserName = userName, Message = message });

            if (CurrentMessage.Count > 100)
            {
                CurrentMessage.RemoveAt(0);
            }
        }
    }
}