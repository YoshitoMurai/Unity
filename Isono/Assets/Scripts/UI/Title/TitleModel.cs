using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Isono.Title
{
	public class TitleModel
	{
		public UserData userData { get; private set; }

		public TitleModel()
		{
			userData = new UserData();
		}
		public void SaveSetting()
		{
			userData.SaveSetting();
		}
		public void SwitchActiveSound()
		{
			userData.setting.onSound = !userData.setting.onSound;
		}
		public void SwitchActiveVib()
		{
			userData.setting.onVibration = !userData.setting.onVibration;
		}
		public void SwitchActiveEnglish()
		{
			userData.setting.onEnglish = !userData.setting.onEnglish;
		}
		public void SwitchActiveEcoMode()
		{
			userData.setting.onEcoMode = !userData.setting.onEcoMode;
		}
	}
}