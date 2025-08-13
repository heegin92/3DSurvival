using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;

    public Button button; // ������ ���Կ� ����� ��ư
    public Image icon; // ������ �������� ǥ���ϱ� ���� �̹���
    public TextMeshProUGUI quantityText; // ������ ������ ǥ���ϱ� ���� �ؽ�Ʈ
    private Outline outline; // ������ ������ �ܰ����� ǥ���ϱ� ���� �ƿ����� ������Ʈ

    public UIInventory inventory; // �κ��丮 UI�� �����ϱ� ���� ����

    public int index; // ������ ������ �ε���
    public bool equipped; // �������� �����Ǿ����� ����
    public int quantity; // �������� ����


    // Start is called before the first frame update
    
    private void Awake()
    {
        outline = GetComponent<Outline>(); // �ƿ����� ������Ʈ�� �����ɴϴ�.
    }


    private void OnEnable()
    {
        outline.enabled = equipped; // �������� �����Ǿ��� �� �ƿ������� Ȱ��ȭ�մϴ�.
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set()
    {
        icon.gameObject.SetActive(true); // �������� Ȱ��ȭ�մϴ�.
        icon.sprite = item.icon; // �������� �������� �����մϴ�.
        quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty; // ������ ������ 1���� ũ�� ������ ǥ���ϰ�, �׷��� ������ �� ���ڿ��� ǥ���մϴ�.
   
        if (outline != null)
        {
            outline.enabled = equipped; // �������� �����Ǿ��� �� �ƿ������� Ȱ��ȭ�մϴ�.
        }
    }

    public void Clear()
    {
        item = null; // �������� ���ϴ�.
        icon.gameObject.SetActive(false); // �������� ��Ȱ��ȭ�մϴ�.
        quantityText.text = string.Empty; // ������ ������ ���ϴ�.
    }

    public void OnClickButton()
    {
        inventory.SelectItem(index);
    }
}
