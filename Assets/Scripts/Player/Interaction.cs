using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractGameObject;
    private IInteractable curInteractable;

    public TextMeshProUGUI promptText;
    private Camera mainCamera;
    public UIInventory uiInventory;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // ⭐ 1. 설치 모드일 때 마우스 위치로 미리보기 오브젝트를 이동
        if (uiInventory.isPlacingItem && uiInventory.previewObject != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, LayerMask.GetMask("Ground")))
            {
                uiInventory.previewObject.transform.position = hit.point;
            }
            return; // 중요: 설치 모드일 때는 아래의 상호작용 로직을 건너뜁니다.
        }

        // ⭐ 2. 기존의 상호작용 로직
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();

                    if (curInteractable != null)
                    {
                        SetPromptText();
                    }
                    else
                    {
                        promptText.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (curInteractGameObject != null)
                {
                    promptText.gameObject.SetActive(false);
                    curInteractGameObject = null;
                    curInteractable = null;
                }
            }
        }
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            // ⭐ 설치 모드일 때만 실행
            if (uiInventory.isPlacingItem)
            {
                // ⭐ 1. 미리보기 오브젝트의 회전 값을 저장합니다.
                Quaternion finalRotation = uiInventory.previewObject.transform.rotation;

                // ⭐ 2. 미리보기 오브젝트를 제거합니다.
                Destroy(uiInventory.previewObject);

                // ⭐ 3. 최종 회전 값을 적용한 새로운 오브젝트를 생성합니다.
                Instantiate(uiInventory.selectedItem.placeablePrefab, uiInventory.previewObject.transform.position, finalRotation);

                // ⭐ 4. 아이템을 설치했으니, 이제 인벤토리에서 아이템을 제거합니다.
                uiInventory.playerInventory.RemoveItem(uiInventory.selectedItem, 1);

                uiInventory.isPlacingItem = false;
                uiInventory.previewObject = null;

     
            }
            else
            {
                // ⭐ 설치 모드가 아닐 때 (기존 상호작용 로직)
                if (curInteractable != null)
                {
                    curInteractable.OnInteract();
                    promptText.gameObject.SetActive(false);
                    curInteractGameObject = null;
                    curInteractable = null;
                }
            }
        }
    }
    public void OnRotateInput(InputAction.CallbackContext context)
    {
        // ⭐ E 키 입력과 마찬가지로, 시작될 때만 실행
        if (context.phase == InputActionPhase.Started)
        {
            // ⭐ 설치 모드일 때만 회전
            if (uiInventory.isPlacingItem && uiInventory.previewObject != null)
            {
                // Y축을 기준으로 90도씩 회전
                uiInventory.previewObject.transform.Rotate(0, 90, 0);
            }
        }
    }

}
