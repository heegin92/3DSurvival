using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering,
    Attacking
}

public class NPC : MonoBehaviour, IDamageable
{
    [Header("Starts")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath;

    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance;
    private AIState aiState;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    public float damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    private float playerDistance;

    public float fieldOfView = 120f;

    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private bool isDying = false; // 👈 사망 상태를 체크하는 새로운 변수

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

    }
    void Start()
    {
        SetState(AIState.Wandering);
    }

    // Update is called once per frame
    void Update()
    {

        // 사망 상태일 때는 아무것도 하지 않도록 조건 추가
        if (isDying) return;
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);

        animator.SetBool("Moving", aiState != AIState.Idle);

        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpadate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
        }
    }

    public void SetState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed;
                agent.isStopped = true;
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = false;
                break;

        }
        animator.speed = agent.speed / walkSpeed; // Adjust animator speed based on agent speed
    }

    void PassiveUpadate()
    {
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        if(playerDistance < detectDistance)
        {
            SetState(AIState.Attacking);
        }
    }

    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return;

        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;

        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30) break;
        }

        return hit.position;
    }

    void AttackingUpdate()
    {
        if (playerDistance < attackDistance && IsPlayerInFieldOfView())
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                CharacterManager.Instance.Player.condition.GetComponent<IDamageable>().TakePhysicalDamage((int)damage);
                animator.speed = 1;
                animator.SetTrigger("Attack");
            }
        }
        else
        {
            if (playerDistance < detectDistance)
            {
                {
                    agent.isStopped = false;
                    NavMeshPath path = new NavMeshPath();
                    if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
                    {
                        agent.SetDestination(CharacterManager.Instance.Player.transform.position);
                    }
                    else
                    {
                        agent.SetDestination(transform.position);
                        agent.isStopped = true;
                        SetState(AIState.Wandering);
                    }
                }
            }
            else
            {
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
            }
        }

        bool IsPlayerInFieldOfView()
        {
            Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            return angle < fieldOfView * 0.5f;
        }

    public void TakePhysicalDamage(int damage)
    {
        if (isDying) return; // 👈 사망 중에는 데미지를 받지 않음

        health -= damage;
        Debug.Log($"데미지 받음! 남은 체력: {health}");

        if (health <= 0)
        {
            Die();
        }

        StartCoroutine(DamageFlash());
    }
    void Die()
    {
        isDying = true; // 👈 사망 상태로 전환
        NPCSpawner spawner = FindObjectOfType<NPCSpawner>();
        if (spawner != null)
        {
            // ⭐ 스포너의 currentNPC 변수를 null로 만듭니다.
            spawner.currentNPC = null;
        }

        // NPC의 이동 및 물리적 충돌 정지
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 사망 애니메이션 재생
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 아이템 드롭
        for (int i = 0; i < dropOnDeath.Length; i++)
        {
            Instantiate(dropOnDeath[i].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }

        // 2초 후 오브젝트 삭제 (애니메이션 재생 시간을 고려)
        Destroy(gameObject, 2.0f);
    }

    IEnumerator DamageFlash()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = new Color(1.0f, 0.6f, 0.6f); 
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.white;
        }
    }
}

