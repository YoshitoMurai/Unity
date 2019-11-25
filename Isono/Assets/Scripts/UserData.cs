using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UserData : MonoBehaviour
{
	public int id { get; set; }
	public int clearStage { get; set; }
	// とりあえずステージ数増えない想定
	public int[] starNum { get; set; } = new int[CommonInfo.STAGE_MAX];
	public Setting setting { get; set; } = new Setting();

	public class Setting
	{
		public bool onSound { get; set; }
		public bool onVibration { get; set; }
		public bool onEnglish { get; set; }
		public bool onEcoMode { get; set; }
		public bool alreadyBuy { get; set; }
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
       
		setting.onSound = (settingFlag & 1000) != 0;
		setting.onVibration = (settingFlag & 0100) != 0;
		setting.onEnglish = (settingFlag & 0010) != 0;
		setting.onEcoMode = (settingFlag & 0001) != 0;
        setting.alreadyBuy = true;
	}
	public void SaveSetting()
	{
		var settingFlag = (setting.onSound ? 1 : 0) << 3
			| (setting.onVibration ? 1 : 0) << 2
			| (setting.onEnglish ? 1 : 0) << 1
			| (setting.onEcoMode ? 1 : 0) << 0;
        PlayerPrefs.SetInt(CommonInfo.SETTING, settingFlag);
	}
}