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
    private Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();

                    // 해결책: curInteractable이 null이 아닐 때만 SetPromptText()를 호출합니다.
                    if (curInteractable != null)
                    {
                        SetPromptText();
                    }
                    else // IInteractable이 없으면 프롬프트를 비활성화합니다.
                    {
                        promptText.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                // Raycast가 아무것도 감지하지 못했을 때
                if (curInteractGameObject != null) // 기존에 감지했던 오브젝트가 있다면
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
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            promptText.gameObject.SetActive(false);
            curInteractGameObject = null;
            curInteractable = null;
        }
    }
}
