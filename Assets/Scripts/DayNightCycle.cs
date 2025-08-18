using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DayNightCycle : MonoBehaviour
{
    // 새로운 하루가 시작될 때 호출될 정적 이벤트
    public static event Action OnNewDay;

    [Range(0f, 1f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.4f;
    private float timeRate;
    public Vector3 noon; // vector 90 0 0

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]

    public AnimationCurve lightingIntensitiyMultiplier;
    public AnimationCurve reflevtionIntensityMultiplier;

    private int lastDay = 0; // 몇 번째 날인지 추적하는 변수

    void Start()
    {
        timeRate = 1f / fullDayLength;
        time = startTime;
    }

    // Update is called once per frame
    // Update is called once per frame
    void Update()
    {
        // 이전 프레임의 time 값을 저장
        float prevTime = time;

        // time을 0.0에서 1.0까지 진행
        time = (time + timeRate * Time.deltaTime) % 1f;

        // 새로운 하루가 시작되었는지 확인
        // ⭐ time이 0.9보다 컸다가 0.1보다 작아질 때를 감지하여 이벤트 호출
        if (prevTime > 0.9f && time < 0.1f)
        {
            OnNewDay?.Invoke(); // 이벤트 호출
            Debug.Log("새로운 하루가 시작되었습니다!");
        }

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensitiyMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflevtionIntensityMultiplier.Evaluate(time);
    }

    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);

        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4f;
        lightSource.color = gradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
        {
            go.SetActive(false);
        }
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }
}
