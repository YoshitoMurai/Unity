using Connect;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Splash : MonoBehaviour
{
    [SerializeField] private Fade fade = default;
    [SerializeField] private CanvasGroup canvasGroup = default;
    void Start()
    {
        fade.FadeIn(canvasGroup, 2.0f);
        Observable.Timer(TimeSpan.FromSeconds(4)).Subscribe(_ =>
        {
            GameSceneManager.Instance.LoadScene(kSceneType.Title);

        }).AddTo(this);
    }
}
