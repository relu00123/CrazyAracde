using Google.Protobuf.Protocol;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Text;

public static class ItemEatHelper
{
    public static void DefaultEatItem(InGameObject eatingObj, CAItemType itemType)
    {
        if (eatingObj is CAPlayer playerObj)
        {
            //Console.WriteLine("Player ate an item (default Implementation).");

            switch (itemType)
            {
                case CAItemType.CaRollerSkate:
                    {
                        Console.WriteLine($"Applying {itemType.ToString()} Effect");
                        playerObj.Stats.IncreaseSpeed();
                    }
                    break;
                case CAItemType.CaStreamMax:
                    {
                        Console.WriteLine($"Applying {itemType.ToString()}Effect");
                        playerObj.Stats.IncreaseWaterPower(toMaxPower: true);
                    }
                    break;
                case CAItemType.CaStreamPotion:
                    {
                        Console.WriteLine($"Applying {itemType.ToString()}Effect");
                        playerObj.Stats.IncreaseWaterPower();
                    }
                    break;
                case CAItemType.CaWaterBomb:
                    {
                        Console.WriteLine($"Applying {itemType.ToString()}Effect");
                        playerObj.Stats.IncreaseMaxBombCount();
                    }
                    break;
            }

        }
    }
}
