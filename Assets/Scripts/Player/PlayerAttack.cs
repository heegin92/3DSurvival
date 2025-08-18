// PlayerAttack.cs
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackDamage = 10f; // 공격 데미지
    public float attackRange = 5f; // 공격 범위
    public float attackRate = 1f;  // 공격 속도
    private float lastAttackTime;
    private ItemData currentEquippedWeapon; // 현재 장착된 무기 아이템 데이터
    public LayerMask enemy;
    public Animator animator;


    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 시 공격
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackRate)
        {
            Attack();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    void Attack()
    {
        lastAttackTime = Time.time;

        // 공격 범위 내의 적을 감지
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemy);
        Debug.Log($"감지된 콜라이더 수: {hitEnemies.Length}");

        foreach (Collider hitCollider in hitEnemies)
        {
            // ⭐ 플레이어 자신이라면 데미지 로직을 건너뜀
            if (hitCollider.gameObject == this.gameObject)
            {
                continue;
            }

            // ⭐ 감지된 콜라이더에서 IDamageable 컴포넌트를 한 번만 찾음
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();

            // ⭐ 컴포넌트가 존재하면 데미지 로직 실행
            if (damageable != null)
            {
                // 최종 데미지 계산 (무기 착용 여부 확인)
                float finalDamage = (currentEquippedWeapon != null) ? currentEquippedWeapon.damage : attackDamage;

                Debug.Log($"대상: {hitCollider.name}, 최종 데미지: {finalDamage}");

                // 데미지 적용
                damageable.TakePhysicalDamage((int)finalDamage);
            }
        }
    }
    public void SetWeapon(ItemData weaponData)
    {
        currentEquippedWeapon = weaponData;
    }
}

   

