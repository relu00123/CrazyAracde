using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public  class StoreManager : JobSerializer
    {
        public static StoreManager Instance { get; } = new StoreManager();

        object _lock = new object();


        public void AddMoney(ClientSession session, int moneyamount)
        {
           S_AddMoney MoneyPacket = new S_AddMoney();
            MoneyPacket.Moneyamount = moneyamount;
            session.Send(MoneyPacket);
        }
          

    }
}
