using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;
    // ������ ���� �迭


    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition; // �������� ����� ��ġ

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription; // ���õ� �������� �̸��� ������ ǥ���� UI ���
    public TextMeshProUGUI selectedStatName;// ���õ� �������� �ɷ�ġ �̸��� ǥ���� UI ���
    public TextMeshProUGUI selectedStatValue; // ���õ� �������� �ɷ�ġ ���� ǥ���� UI ���
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    private PlayerController controller;// �÷��̾� ��Ʈ�ѷ� ����
    private PlayerCondition condition; // �÷��̾� ���� ����

    ItemData selectedItem;
    int selectedItemIndex = 0;

    int curEquipIndex;

    // Start is called before the first frame update
    void Start()
    {
        controller = CharacterManager.Instance.Player.controller; // CharacterManager�� ���� �÷��̾� ��Ʈ�ѷ��� ������
        condition = CharacterManager.Instance.Player.condition; // CharacterManager�� ���� �÷��̾� ���¸� ������
        dropPosition = CharacterManager.Instance.Player.dropPosition; // �÷��̾��� ��� ��ġ�� ������

        controller.inventory = Toggle; // �÷��̾� ��Ʈ�ѷ��� inventory �׼ǿ� Toggle �޼��带 �Ҵ�
        CharacterManager.Instance.Player.addItem += AddItem; // �÷��̾��� ������ �߰� �̺�Ʈ�� AddItem �޼��带 ���

        inventoryWindow.SetActive(false); // �κ��丮 â�� ��Ȱ��ȭ
        slots = new ItemSlot[slotPanel.childCount]; // ���� �迭 �ʱ�ȭ

        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>(); // �� ���Կ� ItemSlot ������Ʈ�� �Ҵ�\
            slots[i].index = i; // �� ������ �ε����� ����
            slots[i].inventory = this; // �� ���Կ� ���� �κ��丮 UI�� �Ҵ�
        }

        ClearSelectedItemWindow(); // ���õ� ������ â �ʱ�ȭ
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ClearSelectedItemWindow()
    {
        selectedItemName.text = string.Empty; // ���õ� ������ �̸� �ʱ�ȭ
        selectedItemDescription.text = string.Empty; // ���õ� ������ ���� �ʱ�ȭ
        selectedStatName.text = string.Empty; // ���õ� ������ �ɷ�ġ �̸� �ʱ�ȭ
        selectedStatValue.text = string.Empty; // ���õ� ������ �ɷ�ġ �� �ʱ�ȭ

        useButton.SetActive(false); // ��� ��ư ��Ȱ��ȭ
        equipButton.SetActive(false); // ���� ��ư ��Ȱ��ȭ
        unequipButton.SetActive(false); // ���� ���� ��ư ��Ȱ��ȭ
        dropButton.SetActive(false); // ��� ��ư ��Ȱ��ȭ
    }

    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false); // �κ��丮 â �ݱ�
        }
        else
        {
            inventoryWindow.SetActive(true); // �κ��丮 â ����
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy; // �κ��丮 â�� �����ִ��� Ȯ��
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData; // �÷��̾��� ������ �����͸� ������

        if (data.canStack) // �������� ���� �������� Ȯ��
        {
            ItemSlot slot = GetItemStack(data); // �ش� �������� ���� ������ ã��
            if (slot != null) // ������ �����ϸ�
            {
                slot.quantity++; // ������ ���� ����
                UpdateUI(); // UI ������Ʈ
                CharacterManager.Instance.Player.itemData = null; // �÷��̾��� ������ �����͸� �ʱ�ȭ
                return; // �޼��� ����
            }
        }

        ItemSlot emptySlot = GetEmptySlot(); // �� ������ ������

        if (emptySlot != null) // �� ������ �����ϸ�
        {
            emptySlot.item = data; // ������ �����͸� ���Կ� �Ҵ�
            emptySlot.quantity = 1; // ������ ������ 1�� ����
            UpdateUI(); // UI ������Ʈ
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data); // �� ������ ������ �������� �ٴڿ� ����
        CharacterManager.Instance.Player.itemData = null; // �÷��̾��� ������ �����͸� �ʱ�ȭ
    }

    void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set(); // ���Կ� ������ ����
            }
            else
            {
                slots[i].Clear(); // ���� ����
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item==data && slots[i].quantity < data.maxStackAmount) // �ش� �����۰� ��ġ�ϰ� �ִ� ���� ������ ���� ���
            {
                return slots[i]; // �ش� �������� ���� ���� ��ȯ
            }
        }
        return null; // ��ġ�ϴ� ������ ������ null ��ȯ
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null) // �������� ���� �� ������ ã��
            {
                return slots[i]; // �� ���� ��ȯ
            }
        }
        return null; // �� ������ ������ null ��ȯ
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
            if (selectedItem.isCoroutine) // �ڷ�ƾ ���������� Ȯ��
            {
                // �ڷ�ƾ ���� �� �α�
                Debug.Log($"�ڷ�ƾ ȸ�� ������ ���! {selectedItem.displayName} ȿ�� ����.");

                // ApplyConsumableEffectsOverTime �ڷ�ƾ�� �����ϰ�, 
                // �������� ��� ȿ�� �迭�� ����
                StartCoroutine(condition.ApplyConsumableEffectsOverTime(
                    selectedItem.consumables,
                    selectedItem.coroutineCount,
                    selectedItem.coroutineInterval)
                );
            }
            else // �Ϲ� �������̶��, ���� ���� ����
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
