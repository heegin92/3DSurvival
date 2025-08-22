using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public GameObject placeButton; // Placeable 타입 아이템을 위한 설치 버튼

    private PlayerController controller;// 플레이어 컨트롤러 참조
    private PlayerCondition condition; // 플레이어 상태 참조
    public Inventory playerInventory;
    private PlayerInput playerInput;

    public bool isPlacingItem = false;
    public GameObject previewObject;

    public ItemData selectedItem;
    int selectedItemIndex = 0;

    int curEquipIndex;

    // Start is called before the first frame update
    void Awake()
    {
        playerInventory = CharacterManager.Instance.Player.GetComponent<Inventory>();
        playerInventory.OnInventoryChanged += UpdateUI;
    }
    void Start()
    {
        controller = CharacterManager.Instance.Player.controller; // CharacterManager를 통해 플레이어 컨트롤러를 가져옴
        condition = CharacterManager.Instance.Player.condition; // CharacterManager를 통해 플레이어 상태를 가져옴
        dropPosition = CharacterManager.Instance.Player.dropPosition; // 플레이어의 드롭 위치를 가져옴

        playerInventory = CharacterManager.Instance.Player.GetComponent<Inventory>(); // ⭐ 추가: Inventory 컴포넌트를 가져옴
        playerInventory.OnInventoryChanged += UpdateUI; // ⭐ 추가: 이벤트 구독

        inventoryWindow.SetActive(false); // 인벤토리 창을 비활성화
        slots = new ItemSlot[slotPanel.childCount]; // 슬롯 배열 초기화

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>(); // 각 슬롯에 ItemSlot 컴포넌트를 할당\
            slots[i].index = i; // 각 슬롯의 인덱스를 설정
            slots[i].inventory = this; // 각 슬롯에 현재 인벤토리 UI를 할당

            // ⭐ ItemSlot의 equipped 변수를 명확하게 false로 초기화
            slots[i].equipped = false;
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
        placeButton.SetActive(false); // 설치 버튼 비활성화 (Placeable 타입 아이템을 위해 추가)
    }

    // ⭐ 설치 모드를 종료하는 함수를 추가
    public void ExitPlacementMode()
    {
        isPlacingItem = false;

        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        // 인벤토리 UI를 다시 엽니다.
        inventoryWindow.SetActive(true);

        // ⭐ Player Input을 다시 활성화하고 커서를 보이게 합니다.
        playerInput.actions.FindActionMap("Player").Enable();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    // ⭐ 인벤토리를 토글하는 함수 (Tab 키 입력 전용)
    public void Toggle(InputAction.CallbackContext context)
    {
        Debug.Log("토글 함수가 실행되었습니다.");

        // This line stays the same
        inventoryWindow.SetActive(!inventoryWindow.activeSelf);
        ClearSelectedItemWindow();

        // ⭐ Check the current state of the inventory window to decide which code to run.
        if (inventoryWindow.activeSelf) // 인벤토리가 '이제' 열렸다면
        {
            Debug.Log("인벤토리가 열립니다. 커서를 보이게 합니다.");

            // 커서를 풀고 보이게 합니다.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Player Input을 비활성화합니다.
            playerInput.actions.FindActionMap("Player").Disable();
        }
        else // 인벤토리가 '이제' 닫혔다면
        {
            Debug.Log("인벤토리가 닫힙니다. 커서를 숨깁니다.");

            // 커서를 잠그고 보이지 않게 합니다.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Player Input을 다시 활성화합니다.
            playerInput.actions.FindActionMap("Player").Enable();
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

        // 능력치 정보 초기화
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n";
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        // ⭐ 1. 모든 버튼을 일단 비활성화하여 깨끗한 상태로 만듭니다.
        useButton.SetActive(false);
        placeButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        dropButton.SetActive(false); // 드롭 버튼도 타입에 따라 제어하는 것이 좋습니다.

        // ⭐ 2. 선택된 아이템의 타입에 따라 필요한 버튼만 활성화합니다.
        switch (selectedItem.type)
        {
            case ItemType.Consumable:
                useButton.SetActive(true);
                break;
            case ItemType.Equipable:
                equipButton.SetActive(!slots[index].equipped);
                unequipButton.SetActive(slots[index].equipped);
                break;
            case ItemType.Placeable:
                placeButton.SetActive(true);
                break;
            default:
                // 그 외 타입(CraftingMaterial, Resource 등)은 버튼을 활성화하지 않습니다.
                break;
        }

        // 모든 아이템은 드롭할 수 있으므로, 항상 활성화합니다.
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

        slots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem);

        SelectItem(selectedItemIndex);
    }

    void UnEquip(int index)
    {
        Debug.Log("UnEquip 함수가 호출되었습니다. 인덱스: " + index);
        slots[index].equipped = false;
        CharacterManager.Instance.Player.equip.UnEquip();
        UpdateUI();

        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    public void OnUnEquipButton()
    {
        Debug.Log("OnUnequipButton 함수가 호출되었습니다.");

        UnEquip(selectedItemIndex);
    }

    public void OnPlaceButton()
    {
        Debug.Log("OnPlaceButton() called");

        if (selectedItem == null || selectedItem.placeablePrefab == null)
        {
            Debug.Log("설치할 아이템이 선택되지 않았거나 프리팹이 할당되지 않았습니다.");
            return;
        }

        isPlacingItem = true;
        inventoryWindow.SetActive(false);

        previewObject = Instantiate(selectedItem.placeablePrefab);

        // ⭐ 이 로그가 뜨는지 확인해봐
        Debug.Log("Preview object instantiated successfully.");

        Rigidbody rb = previewObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        ClearSelectedItemWindow();
    }

    public bool IsInventoryOpen()
    {
        return inventoryWindow.activeSelf;
    }
}