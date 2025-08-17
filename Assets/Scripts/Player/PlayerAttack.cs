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


    void Attack()
    {
        lastAttackTime = Time.time;

        // 공격 범위 내의 적을 감지
        // Physics.OverlapSphere는 지정된 레이어(enemyLayer)의 콜라이더만 감지
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemy);
        Debug.Log($"감지된 콜라이더 수: {hitEnemies.Length}");

        foreach (Collider enemy in hitEnemies)
        {
            // 감지된 콜라이더가 플레이어 자신이 아닌지 확인
            if (enemy.gameObject == this.gameObject)
            {
                continue; // 플레이어 자신이라면 데미지 로직을 건너뜀
            }

            Debug.Log($"감지된 오브젝트: {enemy.name}");

            // IDamageable 인터페이스를 가진 적을 찾음
            IDamageable damageableEnemy = enemy.GetComponent<IDamageable>();
            if (damageableEnemy != null)
            {
                float finalDamage = (currentEquippedWeapon != null) ? currentEquippedWeapon.damage : attackDamage;

                // 이 로그를 추가하여 최종 데미지 값을 확인
                Debug.Log($"대상: {enemy.name}, 최종 데미지: {finalDamage}");

                damageableEnemy.TakePhysicalDamage((int)finalDamage);
            }
        }
    }

    public void SetWeapon(ItemData weaponData)
    {
        currentEquippedWeapon = weaponData;
    }

}