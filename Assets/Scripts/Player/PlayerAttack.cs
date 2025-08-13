// PlayerAttack.cs
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2f; // ���� ����
    public float attackRate = 1f;  // ���� �ӵ�
    private float lastAttackTime;
    private ItemData currentEquippedWeapon; // ���� ������ ���� ������ ������

    void Update()
    {
        // ���콺 ���� ��ư Ŭ�� �� ����
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackRate)
        {
            Attack();
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        // ���� ���� ���� ���� ����
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider enemy in hitEnemies)
        {
            // IDamageable �������̽��� ���� ���� ã��
            IDamageable damageableEnemy = enemy.GetComponent<IDamageable>();
            if (damageableEnemy != null)
            {
                // ���� ������ ������ ������ ������ �������� �ݴϴ�.
                if (currentEquippedWeapon != null)
                {
                    damageableEnemy.TakePhysicalDamage((int)currentEquippedWeapon.damage);
                }
                else
                {
                    // ���Ⱑ ���� ��� �⺻ �������� �ݴϴ�.
                    damageableEnemy.TakePhysicalDamage(10);
                }
            }
        }
    }



}