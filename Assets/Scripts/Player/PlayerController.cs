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
    public LayerMask groundLayerMask; //�� ���̾�
    private Vector2 curMovementInput;
    

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    public float lookSensitivity;
    private float camCurXRot;
    private Vector2 mouseDelta;
    public bool canLook = true; //ī�޶� ȸ�� ���� ����

    public Action inventory;
    private Rigidbody _rigidbody;

    public PlayerCondition condition;

    private void Awake() //��ü�� ������ �� ���� ����, �� ���� ����Ǿ�� �ϴ� �ʱ�ȭ �۾�
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Ŀ�� ���
    }

    // Update is called once per frame
    void FixedUpdate() // ���� ������ FixedUpdate���� ó��
    {
        Move();
    }

    private void LateUpdate() // ī�޶� ȸ���� LateUpdate���� ó��
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
        dir.y =_rigidbody.velocity.y; //���� �ӵ��� y���� ����

        _rigidbody.velocity = dir; //�ӵ� ����
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); //Ŭ���� �ּҰ����� �ּҰ� ��ȯ �ִ밪���� �ִ밪 ��ȯ
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); //y�� ȸ��
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

                _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse); //���� ���� ����
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + transform.up*0.01f, Vector3.down), //������ �Ʒ��� ���� ���
            new Ray(transform.position + (-transform.forward * 0.2f) + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + transform.up * 0.01f, Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                               return true; //���̰� ���� ������ true ��ȯ
            }
        }
        return false; //���̰� ���� ���� ������ false ��ȯ
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
           inventory?.Invoke(); //�κ��丮 �׼��� ȣ��Ǹ� inventory ��������Ʈ�� ����
            ToggleCursor(); //Ŀ�� ��� ���¸� ���
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked; //Ŀ���� ����ִ��� Ȯ��
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked; //Ŀ���� ��װų� ����
        canLook = !toggle; //Ŀ�� ��� ���¿� ���� ī�޶� ȸ�� ���� ���� ����
    }

}
