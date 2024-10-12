using Server.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

public class CAPlayerStats
{
    public float SpeedWeight { get; set; }         // 속도
    public int CurPlaceableBombCount { get; private set; } // 현재 설치할 수 있는 물폭탄의 개수 
    public int MaxBombCount { get; set; }  // 물폭탄 개수
    public int WaterPower { get; set; }    // 물줄기 세기

    public float MaxSpeed { get; set; } = 0.3f;       // 최대 속도
    public int MaxBombCountLimit { get; set; } = 8; // 최대 물폭탄 개수
    public int MaxWaterPower { get; set; } = 7;    // 최대 물줄기 세기

    public CAPlayerStats()
    {
        SpeedWeight = 0;                      // 초기 속도
        MaxBombCount = 1;               // 초기 소유하고 물폭탄 개수
        CurPlaceableBombCount = MaxBombCount;    // 
        WaterPower = 1;                 // 초기 물폭탄 세기 
    }

    public float IncreaseSpeed()
    {
        if (SpeedWeight < MaxSpeed)
            SpeedWeight = SpeedWeight + 0.1f;
        else
            SpeedWeight = MaxSpeed;

        return SpeedWeight;
    }

    public void IncreaseMaxBombCount()
    {
        if (MaxBombCount < MaxBombCountLimit)
        {
            CurPlaceableBombCount++;
            MaxBombCount++;
        }
    }

    public bool IsBombPlaceable() => CurPlaceableBombCount > 0;
   
    public void DecreaseCurBombCount()
    {
        CurPlaceableBombCount--;
    }

    public void IncreaseCurBombCount()
    {
        if (CurPlaceableBombCount + 1 <= MaxBombCount)
            CurPlaceableBombCount++;
    }

    public void IncreaseWaterPower(bool toMaxPower = false)
    {
        if (toMaxPower)
        {
            WaterPower = MaxWaterPower;
            return;
        }

        if (WaterPower < MaxWaterPower)
            WaterPower++;
    }
}