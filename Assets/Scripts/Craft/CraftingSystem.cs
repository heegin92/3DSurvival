using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public Inventory playerInventory; // 플레이어 인벤토리

    public void Craft(CraftingRecipe craftingRecipe)
    {
        // 1. 필요한 모든 재료가 있는지 먼저 확인합니다.
        foreach (var requiredItem in craftingRecipe.requiredItems)
        {
            if (!playerInventory.HasItem(requiredItem.itemData, requiredItem.amount))
            {
                Debug.Log("재료가 부족합니다!");
                return; // 하나라도 재료가 부족하면 제작을 중단합니다.
            }
        }

        // 2. 모든 재료가 충분하면, 인벤토리에서 재료를 제거합니다.
        foreach (var requiredItem in craftingRecipe.requiredItems)
        {
            playerInventory.RemoveItem(requiredItem.itemData, requiredItem.amount);
        }

        // 3. 제작된 아이템을 인벤토리에 추가합니다.
        playerInventory.AddItem(craftingRecipe.outputItem, craftingRecipe.outputCount);

        Debug.Log("울타리 제작 성공!");
    }
}