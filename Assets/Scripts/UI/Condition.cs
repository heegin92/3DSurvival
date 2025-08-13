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
        uiBar.fillAmount = GetPercentage(); // 시작 시 초기값 반영
    }

    // private void Update()
    //{
    //   uiBar.fillAmount = GetPercentage();
    //}

    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue + amount, maxValue);
        uiBar.fillAmount = GetPercentage(); // 값이 변경될 때만 UI 업데이트
    }

    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0);
        uiBar.fillAmount = GetPercentage(); // 값이 변경될 때만 UI 업데이트
    }


    public float GetPercentage()
    {
        return curValue / maxValue;
    }
}
