using Connect.Common;

namespace Connect.Title
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
		public void SwitchActiveRate()
		{
			userData.setting.alreadyRate = !userData.setting.alreadyRate;
		}
	}
}