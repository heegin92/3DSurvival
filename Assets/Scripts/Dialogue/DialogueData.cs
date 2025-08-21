using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public string npcName;

    [TextArea(10, 10)] // 인스펙터 창에서 여러 줄을 입력할 수 있도록 설정
    public string[] sentences;
}
