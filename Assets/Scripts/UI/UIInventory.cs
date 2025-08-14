using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;
    // 아이템 슬롯 배열


    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition; // 아이템을 드롭할 위치

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription; // 선택된 아이템의 이름과 설명을 표시할 UI 요소
    public TextMeshProUGUI selectedStatName;// 선택된 아이템의 능력치 이름을 표시할 UI 요소
    public TextMeshProUGUI selectedStatValue; // 선택된 아이템의 능력치 값을 표시할 UI 요소
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    private PlayerController controller;// 플레이어 컨트롤러 참조
    private PlayerCondition condition; // 플레이어 상태 참조

    ItemData selectedItem;
    int selectedItemIndex = 0;

    int curEquipIndex;

    // Start is called before the first frame update
    void Start()
    {
        controller = CharacterManager.Instance.Player.controller; // CharacterManager를 통해 플레이어 컨트롤러를 가져옴
        condition = CharacterManager.Instance.Player.condition; // CharacterManager를 통해 플레이어 상태를 가져옴
        dropPosition = CharacterManager.Instance.Player.dropPosition; // 플레이어의 드롭 위치를 가져옴

        controller.inventory = Toggle; // 플레이어 컨트롤러의 inventory 액션에 Toggle 메서드를 할당
        CharacterManager.Instance.Player.addItem += AddItem; // 플레이어의 아이템 추가 이벤트에 AddItem 메서드를 등록

        inventoryWindow.SetActive(false); // 인벤토리 창을 비활성화
        slots = new ItemSlot[slotPanel.childCount]; // 슬롯 배열 초기화

        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>(); // 각 슬롯에 ItemSlot 컴포넌트를 할당\
            slots[i].index = i; // 각 슬롯의 인덱스를 설정
            slots[i].inventory = this; // 각 슬롯에 현재 인벤토리 UI를 할당
        }

        ClearSelectedItemWindow(); // 선택된 아이템 창 초기화
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ClearSelectedItemWindow()
    {
        selectedItemName.text = string.Empty; // 선택된 아이템 이름 초기화
        selectedItemDescription.text = string.Empty; // 선택된 아이템 설명 초기화
        selectedStatName.text = string.Empty; // 선택된 아이템 능력치 이름 초기화
        selectedStatValue.text = string.Empty; // 선택된 아이템 능력치 값 초기화

        useButton.SetActive(false); // 사용 버튼 비활성화
        equipButton.SetActive(false); // 장착 버튼 비활성화
        unequipButton.SetActive(false); // 장착 해제 버튼 비활성화
        dropButton.SetActive(false); // 드롭 버튼 비활성화
    }

    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false); // 인벤토리 창 닫기
        }
        else
        {
            inventoryWindow.SetActive(true); // 인벤토리 창 열기
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy; // 인벤토리 창이 열려있는지 확인
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData; // 플레이어의 아이템 데이터를 가져옴

        if (data.canStack) // 아이템이 스택 가능한지 확인
        {
            ItemSlot slot = GetItemStack(data); // 해당 아이템을 가진 슬롯을 찾음
            if (slot != null) // 슬롯이 존재하면
            {
                slot.quantity++; // 아이템 개수 증가
                UpdateUI(); // UI 업데이트
                CharacterManager.Instance.Player.itemData = null; // 플레이어의 아이템 데이터를 초기화
                return; // 메서드 종료
            }
        }

        ItemSlot emptySlot = GetEmptySlot(); // 빈 슬롯을 가져옴

        if (emptySlot != null) // 빈 슬롯이 존재하면
        {
            emptySlot.item = data; // 아이템 데이터를 슬롯에 할당
            emptySlot.quantity = 1; // 아이템 개수를 1로 설정
            UpdateUI(); // UI 업데이트
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data); // 빈 슬롯이 없으면 아이템을 바닥에 던짐
        CharacterManager.Instance.Player.itemData = null; // 플레이어의 아이템 데이터를 초기화
    }

    void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set(); // 슬롯에 아이템 설정
            }
            else
            {
                slots[i].Clear(); // 슬롯 비우기
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item==data && slots[i].quantity < data.maxStackAmount) // 해당 아이템과 일치하고 최대 스택 수보다 적은 경우
            {
                return slots[i]; // 해당 아이템을 가진 슬롯 반환
            }
        }
        return null; // 일치하는 슬롯이 없으면 null 반환
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null) // 아이템이 없는 빈 슬롯을 찾음
            {
                return slots[i]; // 빈 슬롯 반환
            }
        }
        return null; // 빈 슬롯이 없으면 null 반환
    }

    void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n";
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);
        unequipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(true);
    }

    public void OnUseButton()
    {
        if (selectedItem.type == ItemType.Consumable)
        {
            if (selectedItem.isCoroutine) // 코루틴 아이템인지 확인
            {
                // 코루틴 시작 전 로그
                Debug.Log($"코루틴 회복 아이템 사용! {selectedItem.displayName} 효과 시작.");

                // ApplyConsumableEffectsOverTime 코루틴을 시작하고, 
                // 아이템의 모든 효과 배열을 전달
                StartCoroutine(condition.ApplyConsumableEffectsOverTime(
                    selectedItem.consumables,
                    selectedItem.coroutineCount,
                    selectedItem.coroutineInterval)
                );
            }
            else // 일반 아이템이라면, 기존 로직 실행
            {
                for (int i = 0; i < selectedItem.consumables.Length; ++i)
                {
                    switch (selectedItem.consumables[i].type)
                    {
                        case ConsumableType.Health:
                            condition.Health(selectedItem.consumables[i].value);
                            break;
                        case ConsumableType.Hunger:
                            condition.Eat(selectedItem.consumables[i].value);
                            break;
                    }
                }
            }

            RemoveSelectedItem();
        }
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--;

        if (slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null;
            slots[selectedItemIndex].item = null;
            selectedItemIndex = -1;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }
   
    public void OnEquipButton()
    {
        if (slots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    void UnEquip(int index)
    {
        slots[index].equipped = false;
        CharacterManager.Instance.Player.equip.UnEquip();
        UpdateUI();

        if(selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }
}
