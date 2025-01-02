using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenBlackout : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 7.0f;

    public void StartFade()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        var timer = 0f;
        var startColor = fadeImage.color;
        var endColor = new Color(0f, 0f, 0f, 1f);

        while (timer < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = endColor;
    }
}
