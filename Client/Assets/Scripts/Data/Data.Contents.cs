using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{ 
	#region Skill
	[Serializable]
	public class Skill
	{
		public int id;
		public string name;
		public float cooldown;
		public int damage;
		public SkillType skillType;
		public ProjectileInfo projectile;
	}

	public class ProjectileInfo
	{
		public string name;
		public float speed;
		public int range;
		public string prefab;
	}

	[Serializable]
	public class SkillData : ILoader<int, Skill>
	{
		public List<Skill> skills = new List<Skill>();

		public Dictionary<int, Skill> MakeDict()
		{
			Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
			foreach (Skill skill in skills)
				dict.Add(skill.id, skill);
			return dict;
		}
	}
	#endregion

	#region CAItem

	[Serializable]
	public struct CAStoreItemInfo
	{
		public Dictionary<string, string> categories;
	}


	[Serializable]
	public class CAItemData
	{
		public int id;
		public string itemName;
		public CAStoreItemInfo storItemInfo;
		// 나중에 Item Image Path도 추가하자. 
	}

	[Serializable]
	public class CAItemLoader : ILoader<int, CAItemData>
	{
		public List<CAItemData> items = new List<CAItemData>();

		public Dictionary<int, CAItemData> MakeDict()
		{
			Dictionary<int, CAItemData> dict = new Dictionary<int, CAItemData>();
			foreach (CAItemData item in items)
			{
				dict.Add(item.id, item);
			}
			return dict;
		}

		public Dictionary<string, Dictionary<string, List<int>>> MakeCategoryDict()
		{
			Debug.Log("이부분에서 상점UI 전용 대분류 - 소분류 Dictioanry를 추가할 것임");

			Dictionary<string, Dictionary<string, List<int>>> categoryDict = new Dictionary<string, Dictionary<string, List<int>>>();

			foreach (CAItemData item in items)
			{
				foreach (var category in item.storItemInfo.categories)
				{
					string mainCategory = category.Key;
					string subCategory = category.Value;

					if (! categoryDict.ContainsKey(mainCategory))
					{
                        categoryDict[mainCategory] = new Dictionary<string, List<int>>();
					}

					if (!categoryDict[mainCategory].ContainsKey(subCategory))
					{
						categoryDict[mainCategory][subCategory] = new List<int>();
					}

					categoryDict[mainCategory][subCategory].Add(item.id);
				}
			}

			return categoryDict;
		}
	}

    #endregion


    #region Item
    [Serializable]
	public class ItemData
	{
		public int id;
		public string name;
		public ItemType itemType;
		public string iconPath;
	}

	[Serializable]
	public class WeaponData : ItemData
	{
		public WeaponType weaponType;
		public int damage;
	}

	[Serializable]
	public class ArmorData : ItemData
	{
		public ArmorType armorType;
		public int defence;
	}

	[Serializable]
	public class ConsumableData : ItemData
	{
		public ConsumableType consumableType;
		public int maxCount;
	}


	[Serializable]
	public class ItemLoader : ILoader<int, ItemData>
	{
		public List<WeaponData> weapons = new List<WeaponData>();
		public List<ArmorData> armors = new List<ArmorData>();
		public List<ConsumableData> consumables = new List<ConsumableData>();

		public Dictionary<int, ItemData> MakeDict()
		{
			Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();
			foreach (ItemData item in weapons)
			{
				item.itemType = ItemType.Weapon;
				dict.Add(item.id, item);
			}
			foreach (ItemData item in armors)
			{
				item.itemType = ItemType.Armor;
				dict.Add(item.id, item);
			}
			foreach (ItemData item in consumables)
			{
				item.itemType = ItemType.Consumable;
				dict.Add(item.id, item);
			}
			return dict;
		}
	}
	#endregion

	#region Monster

	[Serializable]
	public class MonsterData
	{
		public int id;
		public string name;
		public StatInfo stat;
		public string prefabPath;
	}

	[Serializable]
	public class MonsterLoader : ILoader<int, MonsterData>
	{
		public List<MonsterData> monsters = new List<MonsterData>();

		public Dictionary<int, MonsterData> MakeDict()
		{
			Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
			foreach (MonsterData monster in monsters)
			{
				dict.Add(monster.id, monster);
			}
			return dict;
		}
	}

	#endregion
}