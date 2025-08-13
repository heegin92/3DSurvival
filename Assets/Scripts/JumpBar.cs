using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpPower = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� �÷��̾����� Ȯ��
        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾��� Rigidbody ������Ʈ�� ������
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            // Rigidbody�� �����ϸ� ���� ���� ����
            if (playerRb != null)
            {
                // ForceMode.Impulse�� ����Ͽ� �������� ���� ����
                playerRb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
        }
    }
}
