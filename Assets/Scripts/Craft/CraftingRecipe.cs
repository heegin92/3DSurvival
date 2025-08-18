using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]

public class CraftingRecipe : ScriptableObject
{
    public ItemData outputItem; // 제작 결과물
    public int outputCount; // 결과물 수량

    public ItemAmount[] requiredItems; // 필요한 재료들
}

[System.Serializable]
public class ItemAmount
{
    public ItemData itemData; // 필요한 아이템
    public int amount; // 필요한 수량
}