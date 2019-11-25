using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEngine.SceneManagement;

namespace Isono.Title
{
	public class TitleCtrl : MonoBehaviour
	{
		[SerializeField] private TitleView _view = default;
		private TitleModel _model;
		void Start()
		{
			_model = new TitleModel();
			_view.OpenView(_model.userData);

			_view.onClickStart
				.Subscribe(_ => SceneManager.LoadScene("StageSelect"))
				.AddTo(gameObject);

			_view.onClickSetting
				.Subscribe(_ => _view.SetActiveSettingDialog(true))
				.AddTo(gameObject);

			_view.onClickSettingBack
				.Subscribe(_ => 
				{
					_model.SaveSetting();
					_view.SetActiveSettingDialog(false);
				})
				.AddTo(gameObject);

			_view.onClickSettingDetail
				.Subscribe(settingname => OnSettingButton(settingname))
				.AddTo(gameObject);
		}
		private void OnSettingButton(SettingName settingName)
		{
			switch (settingName)
			{
				case SettingName.Sound:
					_model.SwitchActiveSound();
					_view.SetSoundColor(_model.userData.setting.onSound);
					break;
				case SettingName.Vibration:
					_model.SwitchActiveVib();
					_view.SetVibColor(_model.userData.setting.onVibration);
					break;
				case SettingName.English:
					_model.SwitchActiveEnglish();
					_view.SetEnglishColor(_model.userData.setting.onEnglish);
					break;
				case SettingName.EcoMode:
					_model.SwitchActiveEcoMode();
					_view.SetEcoModeColor(_model.userData.setting.onEcoMode);
					break;
			}
		}
	}
}