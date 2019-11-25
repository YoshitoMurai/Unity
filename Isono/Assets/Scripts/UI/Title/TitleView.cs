using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
namespace Isono.Title
{
	public class TitleView : MonoBehaviour
	{
		[SerializeField] private Button _startButton = default;
		[SerializeField] private Button _settingButton = default;

		public IObservable<Unit> onClickStart => _startButton.OnClickAsObservable();
		public IObservable<Unit> onClickSetting => _settingButton.OnClickAsObservable();


		[Header("Setting")]
		[SerializeField] private GameObject _settingDialog = default;
		[SerializeField] private Button _soundButton = default;
		[SerializeField] private Button _vibrationButton = default;
		[SerializeField] private Button _languageButton = default;
		[SerializeField] private Button _ecomodeButton = default;
		[SerializeField] private Button _backButton = default;
		[SerializeField] private Image _soundButtonImage = default;
		[SerializeField] private Image _vibButtonImage = default;
		[SerializeField] private Image _langButtonImage = default;
		[SerializeField] private Image _ecoButtonImage = default;
		public IObservable<SettingName> onClickSettingDetail;
		public IObservable<Unit> onClickSettingBack => _backButton.OnClickAsObservable();

		[SerializeField] private Color _onSoundColor = default;
		[SerializeField] private Color _onVibrationColor = default;
		[SerializeField] private Color _onEnglishColor = default;
		[SerializeField] private Color _onEcomodeColor = default;
		[SerializeField] private Color _offColor = default;
		public void OpenView(UserData userData)
		{
			onClickSettingDetail = _soundButton.OnClickAsObservable().Select(_ => SettingName.Sound)
				.Merge(_vibrationButton.OnClickAsObservable().Select(_ => SettingName.Vibration)
					, _languageButton.OnClickAsObservable().Select(_ => SettingName.English)
					, _ecomodeButton.OnClickAsObservable().Select(_ => SettingName.EcoMode)
				);
			SetSettingDialog(userData);
		}
		public void SetSettingDialog(UserData userData)
		{
			SetSoundColor(userData.setting.onSound);
			SetVibColor(userData.setting.onVibration);
			SetEnglishColor(userData.setting.onEnglish);
			SetEcoModeColor(userData.setting.onEcoMode);
		}
		public void SetActiveSettingDialog(bool isActive)
		{
			_settingDialog.SetActive(isActive);
		}
		public void SetSoundColor(bool on)
		{
			var colors = _soundButton.colors;
			_soundButtonImage.color = on ? _onSoundColor : _offColor;
		}
		public void SetVibColor(bool on)
		{
			_vibButtonImage.color = on ? _onVibrationColor : _offColor;
		}
		public void SetEnglishColor(bool on)
		{
			_langButtonImage.color = on ? _onEnglishColor : _offColor;
		}
		public void SetEcoModeColor(bool on)
		{
			_ecoButtonImage.color = on ? _onEcomodeColor : _offColor;
		}
	}
	public enum SettingName
	{
		Sound,
		Vibration,
		English,
		EcoMode,
	}
}