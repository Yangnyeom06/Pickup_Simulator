using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InGameBarBasicMove : MonoBehaviour
{
    public Image _background;
    public Image _bar;
    public TextMeshProUGUI _StringState;
    public IntStatValueSO statSO;
    public float lerpSpeed = 5f;
    
    public float widthPerHp = 5f; // HP 1당 픽셀 수 (조절 가능)

    private float targetWidth;

    private void OnEnable()
    {
        statSO.OnCurrentChanged += OnCurrentChanged;
        statSO.OnMaxChanged += OnMaxChanged;
    }

    private void OnDisable()
    {
        statSO.OnCurrentChanged -= OnCurrentChanged;
        statSO.OnMaxChanged -= OnMaxChanged;
    }

    private void Start()
    {
        OnMaxChanged(statSO.max);
        OnCurrentChanged(statSO.current);
    }

    private void OnCurrentChanged(float newCurrent)
    {
        float fullWidth = _background.rectTransform.sizeDelta.x;
        targetWidth = (statSO.current / statSO.max) * fullWidth;

        _StringState.text = $"{Mathf.FloorToInt(statSO.current)} / {Mathf.FloorToInt(statSO.max)}";
    }

    private void OnMaxChanged(float newMax)
    {
        // 새로운 전체 너비 계산 (ex. max 100이면 width = 500)
        float newFullWidth = newMax * widthPerHp;

        // 배경 크기 확장
        var bgRect = _background.rectTransform;
        bgRect.sizeDelta = new Vector2(newFullWidth, bgRect.sizeDelta.y);

        // 마스크 크기도 같이 늘려줌 (Lerp는 현재 너비만 조절하므로 targetWidth만 변경)
        OnCurrentChanged(statSO.current);
    }

    private void Update()
    {
        LerpHpBar();
    }

    private void LerpHpBar()
    {
        RectTransform barRect = _bar.rectTransform;
        float currentWidth = barRect.sizeDelta.x;
        float height = barRect.sizeDelta.y;

        float newWidth = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * lerpSpeed);
        barRect.sizeDelta = new Vector2(newWidth, height);
    }
}