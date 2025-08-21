using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract();
}



public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;
    public int amount = 1; // ⭐ 추가: 아이템 획득 시 수량을 지정하는 변수

    public string GetInteractPrompt()
    {
        string str = $"{data.displayName}\n{data.description}";
        return str;
    }

    public void OnInteract()
    {
        if (this == null) return;
        // ⭐ 수정: Inventory 스크립트의 AddItem 함수를 직접 호출
        // UI가 아닌 실제 인벤토리 데이터에 아이템을 추가합니다.
        CharacterManager.Instance.Player.GetComponent<Inventory>().AddItem(data, amount);

        // 아이템 오브젝트를 월드에서 파괴합니다.
        Debug.Log("아이템을 획득했습니다.");
        Destroy(gameObject);
    }
}
