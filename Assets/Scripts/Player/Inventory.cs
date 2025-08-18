using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    // 실제 아이템을 담을 리스트 (당신이 정의한 Item 클래스를 사용)
    public List<Item> items = new List<Item>();

    // UI에 변경을 알리는 이벤트
    public event Action OnInventoryChanged;

    public bool HasItem(ItemData itemData, int amount)
    {
        Debug.Log("HasItem 함수 호출됨: " + itemData.displayName + " " + amount + "개 필요");
        Debug.Log($"HasItem 함수 호출됨. 인벤토리 아이템 수: {items.Count}");

        foreach (var item in items)
        {
            Debug.Log("인벤토리 아이템: " + item.data.displayName + ", 수량: " + item.count);

            if (item.data.ID == itemData.ID && item.count >= amount)
            {
                Debug.Log("재료가 충분합니다!");
                return true;
            }
        }

        Debug.Log("재료가 부족합니다!");
        return false;
    }

    public void RemoveItem(ItemData itemData, int amount)
    {
        // ⭐ 뒤에서부터 순회하며 아이템을 찾습니다.
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].data.ID == itemData.ID)
            {
                // ⭐ 인벤토리 아이템 수량이 요구량보다 많거나 같을 때만 제거합니다.
                if (items[i].count >= amount)
                {
                    items[i].count -= amount; // 수량 감소

                    if (items[i].count <= 0)
                    {
                        items.RemoveAt(i); // 수량이 0 이하면 리스트에서 제거
                    }

                    // ⭐ 아이템 제거 후 UI 업데이트 이벤트를 호출합니다.
                    OnInventoryChanged?.Invoke();
                    return; // 함수 종료
                }
            }
        }
    }

    public void AddItem(ItemData itemData, int amount)
    {
        // ⭐ 아이템 추가 로직을 구현합니다.
        if (itemData.canStack)
        {
            // 스택 가능한 아이템이라면, 인벤토리에서 해당 아이템을 찾습니다.
            foreach (var item in items)
            {
                if (item.data.ID == itemData.ID)
                {
                    // 아이템을 찾았으면 수량을 추가합니다.
                    item.count += amount;
                    Debug.Log($"실제 인벤토리에 {itemData.displayName} {amount}개 추가됨. 현재 수량: {item.count}");
                    OnInventoryChanged?.Invoke(); // UI 업데이트를 위해 이벤트 호출
                    return; // 함수 종료
                }
            }
        }

        // 기존에 인벤토리에 없는 아이템이거나, 스택 불가능한 아이템이라면 새로 추가합니다.
        items.Add(new Item(itemData, amount));
        Debug.Log($"실제 인벤토리에 {itemData.displayName} {amount}개 추가됨.");
        OnInventoryChanged?.Invoke(); // UI 업데이트를 위해 이벤트 호출
    }
}