using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ItemType
{
    Consumable,
    Equipable,
    Resource
}

public enum ConsumableType
{
    Hunger,
    Health
}

[Serializable]

public  class ItemDataConSumable
{
    public ConsumableType type;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConSumable[] consumables;
    public bool isCoroutine; // 코루틴을 사용할지 여부를 결정하는 변수 추가
    public float coroutineInterval = 0.5f; // 코루틴 간격 변수 추가
    public int coroutineCount = 3; // 코루틴 횟수 변수 추가

    [Header("Equip")]
    public GameObject equipPrefab;
    public float damage;
}

