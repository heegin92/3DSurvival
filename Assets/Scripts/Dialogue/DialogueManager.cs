using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    // 대화 데이터를 담을 변수
    public DialogueData dialogueData;

    // 대화 UI 오브젝트들을 담을 변수
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;
    public GameObject confirmButton;

    // 대화 속도와 타이핑 상태
    public float typingSpeed = 0.05f;
    private bool isTyping = false;
    public bool isDialogueActive = false;

    // 현재 대사 순서를 나타내는 인덱스
    private int currentSentenceIndex = 0;

    // 대화 시스템을 시작하는 함수
    public void StartDialogue()
    {
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        currentSentenceIndex = 0;

        // NPC 이름 설정
        npcNameText.text = dialogueData.npcName;

        // ⭐ DOTween으로 패널 크기 애니메이션
        dialoguePanel.transform.localScale = Vector3.zero;
        dialoguePanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        DisplaySentence();

        // 커서 활성화
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // 다음 대사를 표시하는 함수
    void DisplaySentence()
    {
        if (currentSentenceIndex >= dialogueData.sentences.Length)
        {
            EndDialogue();
            return;
        }

        string sentence = dialogueData.sentences[currentSentenceIndex];
        StartCoroutine(TypeSentence(sentence));
    }

    // ⭐ 타이핑 효과를 위한 코루틴 함수
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = ""; // 텍스트 초기화

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    // 대화 시스템을 종료하는 함수
    public void EndDialogue()
    {
        isDialogueActive = false;
        // ⭐ DOTween으로 패널 크기 애니메이션 후 비활성화
        dialoguePanel.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
        {
            dialoguePanel.SetActive(false);
        });

        // 커서 비활성화
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 확인 버튼을 눌렀을 때 호출되는 함수
    public void OnConfirmButton()
    {
        // ⭐ 타이핑 중이라면 바로 전체 문장을 표시
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = dialogueData.sentences[currentSentenceIndex];
            isTyping = false;
            return;
        }

        currentSentenceIndex++;
        DisplaySentence();
    }
}
