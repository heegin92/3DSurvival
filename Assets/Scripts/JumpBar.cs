using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpPower = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 플레이어인지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어의 Rigidbody 컴포넌트를 가져옴
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            // Rigidbody가 존재하면 위로 힘을 가함
            if (playerRb != null)
            {
                // ForceMode.Impulse를 사용하여 순간적인 힘을 가함
                playerRb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
        }
    }
}
