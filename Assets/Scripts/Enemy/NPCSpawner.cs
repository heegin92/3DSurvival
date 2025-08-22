using UnityEngine;
using System.Collections;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab; // NPC 프리팹
    public Transform[] spawnPoints; // NPC가 생성될 위치

    public GameObject currentNPC; // 현재 씬에 있는 NPC

    void OnEnable()
    {
        // DayNightCycle의 OnNewDay 이벤트에 RespawnOnNewDay 함수를 등록
        DayNightCycle.OnNewDay += RespawnOnNewDay;
        Debug.Log("NPCSpawner 이벤트 구독 시작.");
    }

    void OnDisable()
    {
        // 스크립트가 비활성화되면 이벤트 등록을 해제하여 메모리 누수를 방지
        DayNightCycle.OnNewDay -= RespawnOnNewDay;
        Debug.Log("NPCSpawner 이벤트 구독 해제.");
    }

    void Start()
    {
        SpawnNPC();
    }

    void RespawnOnNewDay()
    {
        // 이제 currentNPC가 null일 때만 리스폰합니다.
        Debug.Log("새로운 하루 이벤트 수신. 현재 NPC 상태 확인 중.");
        if (currentNPC == null)
        {
            Debug.Log("현재 NPC가 없습니다. 리스폰 시작!");
            SpawnNPC();
        }
        else
        {
            Debug.Log("현재 NPC가 존재합니다. 리스폰하지 않습니다.");
        }
    }

    void SpawnNPC()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("리스폰 위치가 설정되지 않았습니다.");
            return;
        }

        // 몬스터를 3마리 생성하도록 for 루프를 사용합니다.
        for (int i = 0; i < 3; i++)
        {
            int randomPointIndex = Random.Range(0, spawnPoints.Length);
            Instantiate(npcPrefab, spawnPoints[randomPointIndex].position, spawnPoints[randomPointIndex].rotation);
            Debug.Log("새로운 NPC가 생성되었습니다.");
        }

        //int randomPointIndex = Random.Range(0, spawnPoints.Length);
       // currentNPC = Instantiate(npcPrefab, spawnPoints[randomPointIndex].position, spawnPoints[randomPointIndex].rotation);
        //Debug.Log("새로운 NPC가 생성되었습니다.");
    }
}
