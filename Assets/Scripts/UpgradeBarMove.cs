using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UpgradeBarMove : MonoBehaviour
{
    public float moveDuration = 1.0f;
    public Vector2 targetPosition = new Vector2(100,0);

    private Vector2 startPosition;
    private float elapsedTime = 0f;
    private bool isMoving = false;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void MoveToTarget()
    {
        startPosition = rectTransform.anchoredPosition;
        elapsedTime = 0f;
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            if (t >= 1f)
            {
                isMoving = false;
            }
        }
    }
}