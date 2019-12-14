using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{
    public class UserData : MonoBehaviour
    {
        public int clearStage { get; private set; }
        public bool onSound { get; private set; }
        public bool onVibration { get; private set; }
        public int selectSkin { get; private set; }
        private int skinData;
        public bool[] isUnsealedSkin { get; private set; }
        #region Singleton

        private static UserData instance;

        public static UserData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (UserData)FindObjectOfType(typeof(UserData));

                    if (instance == null)
                    {
                        Debug.LogError(typeof(UserData) + "is nothing");
                    }
                }

                return instance;
            }
        }

        #endregion Singleton
       
        public void Awake()
        {
            clearStage = PlayerPrefs.GetInt(CommonInfo.CLEAR_STAGE_NUM);
 
            var settingFlag = PlayerPrefs.GetInt(CommonInfo.SETTING);

            onSound = (settingFlag & 0001) != 0;
            onVibration = (settingFlag & 0010) != 0;

            selectSkin = PlayerPrefs.GetInt(CommonInfo.SELECT_SKIN);
            selectSkin = 0;
            skinData = PlayerPrefs.GetInt(CommonInfo.UNSEALED_SKIN);
            skinData = 1;
            isUnsealedSkin = new bool[CommonInfo.SKIN_NUM];
            for (int i = 0; i < CommonInfo.SKIN_NUM; i++)
            {
                isUnsealedSkin[i] = (skinData & (1 << i)) != 0;
            }
            DontDestroyOnLoad(gameObject);
        }
        public void SaveSetting()
        {
            var settingFlag = (onSound ? 1 : 0) << 0
                | (onVibration ? 1 : 0) << 1;
            PlayerPrefs.SetInt(CommonInfo.SETTING, settingFlag);
            Save();
        }
        public void SwitchSound()
        {
            onSound = !onSound;
            SaveSetting();
        }
        public void SwitchVib()
        {
            onVibration = !onVibration;
            SaveSetting();
        }
        public void SetClearStage(int num)
        {
            PlayerPrefs.SetInt(CommonInfo.CLEAR_STAGE_NUM, num);
            clearStage = num;
            Save();
        }
        public void SetSkin(int num)
        {
            selectSkin = num;
            PlayerPrefs.SetInt(CommonInfo.SELECT_SKIN, num);
            Save();
        }
        public void AddSkin(int num)
        {
            if (num == -1) return;
            skinData +=  1  << num;
            isUnsealedSkin[num] = true;
            PlayerPrefs.SetInt(CommonInfo.UNSEALED_SKIN, skinData);
            Save();
        }
        private void Save()
        {
            PlayerPrefs.Save();
        }
    }
}