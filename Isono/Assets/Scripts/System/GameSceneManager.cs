using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Connect
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance;
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;
            DontDestroyOnLoad(Instance);
        }

        public void LoadScene(kSceneType type)
        {
            FadeManager.Instance.LoadScene(type,1.0f);
        }
    }
    public enum kSceneType
    {
        Title = 0,
        Game = 1,
    }
}