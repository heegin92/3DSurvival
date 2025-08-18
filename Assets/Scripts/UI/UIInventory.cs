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
    private Inventory playerInventory;

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

        playerInventory = CharacterManager.Instance.Player.GetComponent<Inventory>(); // ⭐ 추가: Inventory 컴포넌트를 가져옴
        playerInventory.OnInventoryChanged += UpdateUI; // ⭐ 추가: 이벤트 구독

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



    public void UpdateUI()
    {
        // 1. 먼저 모든 기존 UI 슬롯을 초기화하여 깨끗한 상태로 만듭니다.
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Clear();
        }

        // 2. 인벤토리의 실제 아이템 데이터로 UI 슬롯을 채웁니다.
        for (int i = 0; i < playerInventory.items.Count; i++)
        {
            // UI 슬롯 배열의 범위를 벗어나지 않도록 확인합니다.
            if (i < slots.Length)
            {
                Item currentItem = playerInventory.items[i];

                slots[i].item = currentItem.data;
                slots[i].quantity = currentItem.count;
                slots[i].Set();
            }
        }
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
