using Google.Protobuf.Protocol;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Text;

 

public interface IPlayerEatItem
{
    void EatItem(InGameObject eatingObj, CAItemType itemType);
}