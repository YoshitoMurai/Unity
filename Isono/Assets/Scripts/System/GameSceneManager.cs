using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace IsonoGame
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance;

        public enum kSceneType
        {
            Game = 0,
            Title = 1,
            StageSelect = 2,
        }

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
            SceneManager.LoadScene((int)type);
        }
    }
}