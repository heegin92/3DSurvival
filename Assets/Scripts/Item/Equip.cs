using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equip : MonoBehaviour
{
    // ItemData 변수에 Equipment.cs 스크립트가 아이템 데이터를 할당합니다.
    public ItemData itemData;

    // 공격 범위를 설정하는 변수 (Inspector에서 설정)
    public float attackRange = 2f;

    // 공격할 대상을 구분하는 레이어 마스크 (Inspector에서 설정)
    public LayerMask hitLayers;

    // OnAttackInput() 함수는 Equipment.cs 스크립트에서 호출됩니다.
    public void OnAttackInput()
    {
        // 플레이어의 위치를 중심으로 지정된 범위와 레이어에 있는 콜라이더를 감지
        Collider[] hitObjects = Physics.OverlapSphere(
            transform.position,
            attackRange,
            hitLayers
        );

        // 감지된 모든 오브젝트를 순회
        foreach (Collider hitObject in hitObjects)
        {
            // IDamageable 인터페이스를 가진 오브젝트인지 확인
            IDamageable damageable = hitObject.GetComponent<IDamageable>();

            // IDamageable이 있다면 데미지를 줍니다.
            if (damageable != null)
            {
                // itemData에 저장된 데미지 값으로 공격
                damageable.TakePhysicalDamage((int)itemData.damage);

                // 디버그 로그 추가 (선택 사항)
                Debug.Log($"플레이어가 {hitObject.name}에게 {(int)itemData.damage} 데미지를 입혔습니다.");
            }
        }
    }
}
