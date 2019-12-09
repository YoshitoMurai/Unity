using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{
    public class UserData : MonoBehaviour
    {
        public int clearStage { get; set; }
        public bool onSound { get; private set; }
        public bool onVibration { get; private set; }
        public int selectMaterial { get; private set; }
        private int material;
        public bool[] sealedMaterial { get; private set; }
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

            selectMaterial = PlayerPrefs.GetInt(CommonInfo.SELECT_SKIN);
            selectMaterial = 0;
            material = PlayerPrefs.GetInt(CommonInfo.UNSEALED_SKIN);
            material = 4095;
            sealedMaterial = new bool[CommonInfo.SKIN_NUM];
            for (int i = 0; i < CommonInfo.SKIN_NUM; i++)
            {
                sealedMaterial[i] = (material & (1 << i)) != 0;
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
            Save();
        }
        public void SetMaterial(int num)
        {
            selectMaterial = num;
            PlayerPrefs.SetInt(CommonInfo.SELECT_SKIN, num);
            Save();
        }
        public void AddMaterial(int num)
        {
            material +=  1  << num;
            sealedMaterial[num] = true;
            PlayerPrefs.SetInt(CommonInfo.UNSEALED_SKIN, material);
            Save();
        }
        private void Save()
        {
            PlayerPrefs.Save();
        }
    }
}