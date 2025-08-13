using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    public LayerMask groundLayerMask; //땅 레이어
    private Vector2 curMovementInput;
    

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    public float lookSensitivity;
    private float camCurXRot;
    private Vector2 mouseDelta;
    public bool canLook = true; //카메라 회전 가능 여부

    public Action inventory;
    private Rigidbody _rigidbody;

    public PlayerCondition condition;

    private void Awake() //객체가 생성된 후 가장 먼저, 한 번만 실행되어야 하는 초기화 작업
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //커서 잠금
    }

    // Update is called once per frame
    void FixedUpdate() // 물리 연산은 FixedUpdate에서 처리
    {
        Move();
    }

    private void LateUpdate() // 카메라 회전은 LateUpdate에서 처리
    {
        if ( canLook)
        {
            CameraLook();
        }
        
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y =_rigidbody.velocity.y; //현재 속도의 y값을 유지

        _rigidbody.velocity = dir; //속도 설정
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); //클램프 최소값에선 최소값 반환 최대값에선 최대값 반환
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); //y축 회전
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            mouseDelta = context.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {

                _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse); //위로 힘을 가함
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + transform.up*0.01f, Vector3.down), //위에서 아래로 레이 쏘기
            new Ray(transform.position + (-transform.forward * 0.2f) + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + transform.up * 0.01f, Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                               return true; //레이가 땅에 닿으면 true 반환
            }
        }
        return false; //레이가 땅에 닿지 않으면 false 반환
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
           inventory?.Invoke(); //인벤토리 액션이 호출되면 inventory 델리게이트를 실행
            ToggleCursor(); //커서 잠금 상태를 토글
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked; //커서가 잠겨있는지 확인
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked; //커서를 잠그거나 해제
        canLook = !toggle; //커서 잠금 상태에 따라 카메라 회전 가능 여부 변경
    }

}
