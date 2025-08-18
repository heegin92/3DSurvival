using UnityEngine;
using System;

// 이 클래스는 단순한 데이터 구조체이므로 ScriptableObject를 상속받지 않습니다.
// [Serializable] 속성만 사용해 인스펙터에 표시되도록 합니다.
[Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

public enum ItemType
{
    Consumable,
    Equipable,
    Resource,
    CraftingMaterial
}

public enum ConsumableType
{
    Hunger,
    Health,
    Water
}

// ItemData는 모든 아이템 정보를 담는 주요 ScriptableObject입니다.
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    public int ID;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;
    public bool isCoroutine;
    public float coroutineInterval = 0.5f;
    public int coroutineCount = 3;

    [Header("Equip")]
    public GameObject equipPrefab;
    public float damage;
}

[System.Serializable]
public class Item
{
    public ItemData data;
    public int count;

    public Item(ItemData itemData, int amount)
    {
        data = itemData;
        count = amount;
    }
}

