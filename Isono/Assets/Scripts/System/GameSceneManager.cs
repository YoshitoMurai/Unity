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
            if(FadeManager.Instance != null) FadeManager.Instance.LoadScene(type,0.5f);
            else SceneManager.LoadScene((int)type);
        }
    }
    public enum kSceneType
    {
        Splash = 0,
        Title = 1,
        Game = 2,
    }
}