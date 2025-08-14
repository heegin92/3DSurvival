using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equip : MonoBehaviour
{
    // ItemData ������ Equipment.cs ��ũ��Ʈ�� ������ �����͸� �Ҵ��մϴ�.
    public ItemData itemData;

    // ���� ������ �����ϴ� ���� (Inspector���� ����)
    public float attackRange = 2f;

    // ������ ����� �����ϴ� ���̾� ����ũ (Inspector���� ����)
    public LayerMask hitLayers;

    // OnAttackInput() �Լ��� Equipment.cs ��ũ��Ʈ���� ȣ��˴ϴ�.
    public void OnAttackInput()
    {
        // �÷��̾��� ��ġ�� �߽����� ������ ������ ���̾ �ִ� �ݶ��̴��� ����
        Collider[] hitObjects = Physics.OverlapSphere(
            transform.position,
            attackRange,
            hitLayers
        );

        // ������ ��� ������Ʈ�� ��ȸ
        foreach (Collider hitObject in hitObjects)
        {
            // IDamageable �������̽��� ���� ������Ʈ���� Ȯ��
            IDamageable damageable = hitObject.GetComponent<IDamageable>();

            // IDamageable�� �ִٸ� �������� �ݴϴ�.
            if (damageable != null)
            {
                // itemData�� ����� ������ ������ ����
                damageable.TakePhysicalDamage((int)itemData.damage);

                // ����� �α� �߰� (���� ����)
                Debug.Log($"�÷��̾ {hitObject.name}���� {(int)itemData.damage} �������� �������ϴ�.");
            }
        }
    }
}
