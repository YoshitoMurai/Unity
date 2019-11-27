using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using IsonoGame.Common;

namespace Isono.Title
{
    public class TitleView : MonoBehaviour
	{
		[SerializeField] private Button _startButton = default;
		[SerializeField] private Button _settingButton = default;

		public IObservable<Unit> onClickStart => _startButton.OnClickAsObservable();
		public IObservable<Unit> onClickSetting => _settingButton.OnClickAsObservable();


		[Header("Setting[0:Sound 1:Vibration 2:Rate]")]
		[SerializeField] private Button _settingDialog = default;
		[SerializeField] private Button[] _settingDetailButton = default;
		[SerializeField] private Image[] _settingButtonImage = default;
        [SerializeField] private Sprite[] _settingOnSprite = default;
        [SerializeField] private Sprite[] _settingOffSprite = default;
        public IObservable<SettingName> onClickSettingDetail;

		public void OpenView(UserData userData)
		{
			onClickSettingDetail = _settingDetailButton[0].OnClickAsObservable().Select(_ => SettingName.Sound)
				.Merge(_settingDetailButton[1].OnClickAsObservable().Select(_ => SettingName.Vibration)
					, _settingDetailButton[2].OnClickAsObservable().Select(_ => SettingName.Rate)
				);

            _settingDialog.OnClickAsObservable().Subscribe(_=>_settingDialog.gameObject.SetActive(false)).AddTo(gameObject);

            SetSettingDialog(userData);

			
		}
		private void SetSettingDialog(UserData userData)
		{
            SetSettingButton(SettingName.Sound, userData.setting.onSound);
            SetSettingButton(SettingName.Vibration, userData.setting.onVibration);
        }
        public void SetSettingButton(SettingName index, bool on)
        {
            _settingButtonImage[(int)index].sprite = on ? _settingOnSprite[(int)index] : _settingOffSprite[(int)index];
        }
		public void SetActiveSettingDialog(bool isActive)
		{
			_settingDialog.gameObject.SetActive(isActive);
		}
	}
	public enum SettingName
	{
		Sound,
		Vibration,
		Rate,
	}
}