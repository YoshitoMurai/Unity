using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Connect
{
    public class Fade : MonoBehaviour
    {
        public void FadeOut(CanvasGroup canvasGroup, float duration,  GraphicRaycaster gr = null)
        {
            if (gr != null) gr.enabled = true;
            canvasGroup.DOFade(0.0F, duration).
                OnComplete(() =>
                {
                    canvasGroup.blocksRaycasts = false;
                    if (gr != null) gr.enabled = false;
                });
        }

        public void FadeIn(CanvasGroup canvasGroup, float duration, GraphicRaycaster gr = null)
        {
            if(gr != null)gr.enabled = true;
            canvasGroup.DOFade(1.0F, duration).OnComplete(() =>
            {
                canvasGroup.blocksRaycasts = true;
                if (gr != null) gr.enabled = false;
            });
        }
    }
}