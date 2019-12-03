using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{
    public class UserData
    {
        public int id { get; set; }
        public int clearStage { get; set; }
        public int[] starNum { get; set; } = new int[CommonInfo.STAGE_MAX];
        public Setting setting { get; set; } = new Setting();

        public class Setting
        {
            public bool onSound { get; set; }
            public bool onVibration { get; set; }
            public bool alreadyRate { get; set; }
        }
        public UserData()
        {
            id = 0;
            clearStage = 3;
            for (int i = 0; i < clearStage; i++)
            {
                starNum[i] = 2;
            }

            var settingFlag = PlayerPrefs.GetInt(CommonInfo.SETTING);

            setting.onSound = (settingFlag & 0001) != 0;
            setting.onVibration = (settingFlag & 0010) != 0;
            setting.alreadyRate = (settingFlag & 0100) != 0;
        }
        public void SaveSetting()
        {
            var settingFlag = (setting.onSound ? 1 : 0) << 0
                | (setting.onVibration ? 1 : 0) << 1
                | (setting.alreadyRate ? 1 : 0) << 2;
            PlayerPrefs.SetInt(CommonInfo.SETTING, settingFlag);
        }
    }
}