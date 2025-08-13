// PlayerAttack.cs
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2f; // 공격 범위
    public float attackRate = 1f;  // 공격 속도
    private float lastAttackTime;
    private ItemData currentEquippedWeapon; // 현재 장착된 무기 아이템 데이터

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 시 공격
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackRate)
        {
            Attack();
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        // 공격 범위 내의 적을 감지
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider enemy in hitEnemies)
        {
            // IDamageable 인터페이스를 가진 적을 찾음
            IDamageable damageableEnemy = enemy.GetComponent<IDamageable>();
            if (damageableEnemy != null)
            {
                // 현재 장착된 무기의 데미지 값으로 데미지를 줍니다.
                if (currentEquippedWeapon != null)
                {
                    damageableEnemy.TakePhysicalDamage((int)currentEquippedWeapon.damage);
                }
                else
                {
                    // 무기가 없을 경우 기본 데미지를 줍니다.
                    damageableEnemy.TakePhysicalDamage(10);
                }
            }
        }
    }



}