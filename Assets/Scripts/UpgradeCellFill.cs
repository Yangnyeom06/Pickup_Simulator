using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpgradeCellFill : MonoBehaviour
{
    [SerializeField] private RectTransform fillImageRect;
    [SerializeField] private float fillDuration = 0.3f; // 게이지 한 칸 채우는 시간
    public float maxWidth = 100f; // 완전히 찼을 때의 너비

    private Coroutine fillRoutine;

    public void SetMaxWidth(float width)
    {
        maxWidth = width;
    }

    public void SetFillAmount(float amount)
    {
        amount = Mathf.Clamp01(amount);
        float targetWidth = maxWidth * amount;

        StopAllCoroutines();
        StartCoroutine(SmoothFill(targetWidth));
    }

    private IEnumerator SmoothFill(float targetWidth)
    {
        float current = fillImageRect.sizeDelta.x;
        while (Mathf.Abs(current - targetWidth) > 0.1f)
        {
            current = Mathf.Lerp(current, targetWidth, Time.deltaTime * 10f);
            fillImageRect.sizeDelta = new Vector2(current, fillImageRect.sizeDelta.y);
            yield return null;
        }
        fillImageRect.sizeDelta = new Vector2(targetWidth, fillImageRect.sizeDelta.y);
    }
}
