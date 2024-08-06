using Google.Protobuf.Protocol;
using Server.Game.Area.Lobby;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class ChattingManager
    {
        public static ChattingManager Instance { get; } = new ChattingManager();

        object _lock = new object();


        public void HandleChattingPacket(C_Chatting chattingPacket)
        {
            GameLogic.Instance.Push(() =>
            {
                UserManager.Instance.Push(BroadCastChat, chattingPacket);
            });
        }


        public void BroadCastChat(C_Chatting RecviedChat)
        {
            S_Chatting SendChat = new S_Chatting
            {
                Name = RecviedChat.Name,
                Chat = RecviedChat.Chat
            };
            
            lock (_lock)
            {

                foreach(KeyValuePair<int, PlayerInfo> kvp in UserManager.Instance.Players)
                {
                    // 나중에 확장성 있게 고쳐야 한다.
                    // 지금은 로비에서만 채팅을 친다고 가정 
                    // 패킷에다가 서버스테이트가 무엇인지, InGame이라면 
                    // Room Number에 따라 해당 룸에서만 BroadCast되도록 해야할듯
                    if (kvp.Value.Session._serverState == PlayerServerState.ServerStateLobby)
                    {
                        kvp.Value.Session.Send(SendChat);
                    }
                }
            }
        }
    }
}
