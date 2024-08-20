﻿using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.Skill> SkillDict { get; private set; } = new Dictionary<int, Data.Skill>();
    public Dictionary<int, Data.ItemData> ItemDict { get; private set; } = new Dictionary<int, Data.ItemData>();
    public Dictionary<int, Data.MonsterData> MonsterDict { get; private set; } = new Dictionary<int, Data.MonsterData>();

    public Dictionary<int, Data.CAItemData> CAItemDict { get; private set; } = new Dictionary<int, Data.CAItemData>();

    public Dictionary<string, Dictionary<string, List<int>>> CAItemCategoryDict { get; private set; } = new Dictionary<string, Dictionary<string, List<int>>>();

    public Dictionary<MapType, Sprite> MapPreviewImageDict { get; private set; } = new Dictionary<MapType, Sprite>();

    
    public void Init()
    {
        SkillDict = LoadJson<Data.SkillData, int, Data.Skill>("SkillData").MakeDict();
        ItemDict = LoadJson<Data.ItemLoader, int, Data.ItemData>("ItemData").MakeDict();
        MonsterDict = LoadJson<Data.MonsterLoader, int, Data.MonsterData>("MonsterData").MakeDict();

        Data.CAItemLoader CA_ItemLoader = LoadJson<Data.CAItemLoader, int, Data.CAItemData>("CAItemData");
        CAItemDict = CA_ItemLoader.MakeDict();
        CAItemCategoryDict = CA_ItemLoader.MakeCategoryDict();

       LoadMapPreviewImage();
	}

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.text);
	}

    private void LoadMapPreviewImage()
    {
        Sprite[] NormalMapImages = Resources.LoadAll<Sprite>("Textures/GameRoom/MapPreview/Normal");
        Sprite[] MonsterMapImages = Resources.LoadAll<Sprite>("Textures/GameRoom/MapPreview/Monster");
        Sprite[] mapImages = NormalMapImages.Concat(MonsterMapImages).ToArray();

        for (int i = 0; i < mapImages.Length; ++i)
        {
            if (Enum.TryParse(mapImages[i].name, out MapType mapType))
            {
                MapPreviewImageDict.Add(mapType, mapImages[i]);
            }
        }
    }
}
