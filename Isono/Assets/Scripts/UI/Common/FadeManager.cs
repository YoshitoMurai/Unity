using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Connect
{
    public class FadeManager : MonoBehaviour
    {
        [SerializeField] private Image _fadeImage = default;
        [SerializeField] private GraphicRaycaster graphicRaycaster = default;

        #region Singleton

        private static FadeManager instance;

        public static FadeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (FadeManager)FindObjectOfType(typeof(FadeManager));

                    if (instance == null)
                    {
                        Debug.LogError(typeof(FadeManager) + "is nothing");
                    }
                }

                return instance;
            }
        }

        #endregion Singleton


        private float fadeAlpha = 0;
        private bool isFading = false;


        public void Awake()
        {
            if (this != Instance)
            {
                Destroy(gameObject);
                return;
            }
            graphicRaycaster.enabled = false;
            DontDestroyOnLoad(gameObject);
        }
        public void LoadScene(kSceneType scene, float interval)
        {
            StartCoroutine(TransScene(scene, interval));
        }
        private IEnumerator TransScene(kSceneType scene, float interval)
        {
            graphicRaycaster.enabled = true;
            //だんだん暗く .
            this.isFading = true;
            float time = 0;
            while (time <= interval)
            {
                this.fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
                _fadeImage.color = new Color(0,0,0,fadeAlpha);
                time += Time.deltaTime;
                yield return 0;
            }

            //シーン切替 .
            SceneManager.LoadScene((int)scene);

            //だんだん明るく .
            time = 0;
            while (time <= interval)
            {
                this.fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
                _fadeImage.color = new Color(0, 0, 0, fadeAlpha);
                time += Time.deltaTime;
                yield return 0;
            }
            _fadeImage.color = new Color(0, 0, 0, 0);
            this.isFading = false;
            graphicRaycaster.enabled = false;
        }
    }
}