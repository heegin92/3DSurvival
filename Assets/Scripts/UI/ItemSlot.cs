using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;

    public Button button; // 아이템 슬롯에 연결된 버튼
    public Image icon; // 아이템 아이콘을 표시하기 위한 이미지
    public TextMeshProUGUI quantityText; // 아이템 개수를 표시하기 위한 텍스트
    private Outline outline; // 아이템 슬롯의 외곽선을 표시하기 위한 아웃라인 컴포넌트

    public UIInventory inventory; // 인벤토리 UI를 참조하기 위한 변수

    public int index; // 아이템 슬롯의 인덱스
    public bool equipped; // 아이템이 장착되었는지 여부
    public int quantity; // 아이템의 개수


    // Start is called before the first frame update
    
    private void Awake()
    {
        outline = GetComponent<Outline>(); // 아웃라인 컴포넌트를 가져옵니다.
    }


    private void OnEnable()
    {
        outline.enabled = equipped; // 아이템이 장착되었을 때 아웃라인을 활성화합니다.
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
        icon.enabled = true;
        quantityText.enabled = true;

        icon.sprite = item.icon;
        quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty;

        if (outline != null)
        {
            outline.enabled = equipped;
        }
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
        icon.enabled = false;
        quantityText.enabled = false;

        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void OnClickButton()
    {
        inventory.SelectItem(index);
    }
}
