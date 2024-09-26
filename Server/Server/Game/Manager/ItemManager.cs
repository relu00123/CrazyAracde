using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

public class ItemManager
{
    private static ItemManager _instance;
    private static readonly object _lock = new object();

    public float _itemSpawnChance = 0.5f;  // 50%의 확률로 아이템 생성

    private int[] itemWeights = { 2, 1, 2, 2};
    private CAItemType[] itemTypes =
    {
        CAItemType.CaRollerSkate,
        CAItemType.CaStreamMax,
        CAItemType.CaStreamPotion,
        CAItemType.CaWaterBomb,
    };

    private Random random;

    private ItemManager()
    {
        random = new Random();
    }

    public static ItemManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = new ItemManager();
            }

            return _instance;
        }
    }

    // 아이템을 확률적으로 선택하는 함수
    public CAItemType GetRandomItemType()
    {
        int totalWeight = 0;
        foreach (int weight in itemWeights)
        {
            totalWeight += weight; // 가중치의 합 계산
        }

        int randomValue = random.Next(0, totalWeight); // 0 부터 totalweight - 1까지의 난수 생성
        int cumulativeWeight = 0;

        // 가중치에 따른 아이템 선택
        for (int i = 0; i < itemWeights.Length; i++)
        {
            cumulativeWeight += itemWeights[i];
            if (randomValue < cumulativeWeight)
            {
                return itemTypes[i];
            }
        }

        return CAItemType.CaRollerSkate;
    }

    public bool ShouldSpawnItem()
    {
        float randValue = (float)random.NextDouble();
        return randValue < _itemSpawnChance;
    }
}