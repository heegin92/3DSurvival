using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float maxValue;
    public float startValue;
    public float passiveValue;
    public Image uiBar;

    private void Start()
    {
        curValue = startValue;
        uiBar.fillAmount = GetPercentage(); // ���� �� �ʱⰪ �ݿ�
    }

    // private void Update()
    //{
    //   uiBar.fillAmount = GetPercentage();
    //}

    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue + amount, maxValue);
        uiBar.fillAmount = GetPercentage(); // ���� ����� ���� UI ������Ʈ
    }

    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0);
        uiBar.fillAmount = GetPercentage(); // ���� ����� ���� UI ������Ʈ
    }


    public float GetPercentage()
    {
        return curValue / maxValue;
    }
}
