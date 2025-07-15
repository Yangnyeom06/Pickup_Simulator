using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class FadeInOut : MonoBehaviour
{
    public GameObject panel;
    public float fadeTime = 1.0f;
    public float delayTime = 1.0f;
    private Image panelImage;
    public AnimationCurve fadeCurve;



    void Start()
    {
        panelImage = panel.GetComponent<Image>();
    }

    public void StartFadeInAndOut()
    {
        StartCoroutine(FadeInOutSequence());
    }

    public void StartFadeIn()
    {
        StartCoroutine(Fade(1, 0));

    }

    public void StartFadeOut()
    {
        StartCoroutine(Fade(0, 1));
    }

    private IEnumerator FadeInOutSequence()
    {
        Debug.Log("눌림");
        panel.SetActive(true);
        yield return StartCoroutine(Fade(0, 1));
        yield return new WaitForSeconds(delayTime);
        yield return StartCoroutine(Fade(1, 0));
        panel.SetActive(false);
    }

    private IEnumerator Fade(float start, float end)
    {
        float currentTime = 0.0f;
        float percent = 0.0f;

        while (percent < 1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / fadeTime;

            Color color = panelImage.color;
            color.a = Mathf.Lerp(start, end, fadeCurve.Evaluate(percent));
            panelImage.color = color;

            yield return null;
        }
    }
}
