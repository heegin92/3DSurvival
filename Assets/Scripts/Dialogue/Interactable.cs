using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public DialogueData dialogueData;
    public DialogueManager dialogueManager;

    //범위안에 플레이어가 있는지 체크
    private bool isPlayerInRange = false;

    private bool hasSpoken = false; // 대화가 시작되었는지 여부

    //플레이어가 범위안에 들어오면 호출되는 함수
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // 플레이어에게 '상호작용' 메시지를 표시
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // 플레이어에게 '상호작용' 메시지를 숨김
        }
    }

    public void OnInteract()
    {
        // ⭐ 이 줄을 추가합니다.
        Debug.Log("OnInteract 함수가 호출되었습니다.");
        if (isPlayerInRange && !dialogueManager.isDialogueActive)
        {
            dialogueManager.StartDialogue();
            hasSpoken = true; // 대화 시작 시 true로 설정
        }
    }
}
