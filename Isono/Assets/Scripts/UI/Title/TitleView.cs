using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEngine.SceneManagement;
using Connect.Common;

namespace Connect.Title
{
    public class TitleView : MonoBehaviour
	{
        

		[SerializeField] private Button _startButton = default;
		[SerializeField] private Button _settingButton = default;
        [SerializeField] private Button _noAdsButton = default;
        [SerializeField] private Button _instagramButton = default;
        [SerializeField] private Button _twitterButton = default;


		public IObservable<Unit> onClickStart => _startButton.OnClickAsObservable();
		public IObservable<Unit> onClickSetting => _settingButton.OnClickAsObservable();
        public IObservable<Unit> onClickNoAds => _settingButton.OnClickAsObservable();
        public IObservable<Unit> onClickInstagram => _instagramButton.OnClickAsObservable();
        public IObservable<Unit> onClickTwitter => _twitterButton.OnClickAsObservable();

        [Header("Setting[0:Sound 1:Vibration 2:Rate]")]
		[SerializeField] private Button _settingDialog = default;
		[SerializeField] private Button[] _settingDetailButton = default;
		[SerializeField] private Image[] _settingButtonImage = default;
        [SerializeField] private Sprite[] _settingOnSprite = default;
        [SerializeField] private Sprite[] _settingOffSprite = default;
        public IObservable<SettingName> onClickSettingDetail;

        [SerializeField] GraphicRaycaster _graphicRaycaster = default;
		public void OpenView(UserData userData)
		{
			onClickSettingDetail = _settingDetailButton[0].OnClickAsObservable().Select(_ => SettingName.Sound)
				.Merge(_settingDetailButton[1].OnClickAsObservable().Select(_ => SettingName.Vibration)
					, _settingDetailButton[2].OnClickAsObservable().Select(_ => SettingName.Rate)
				);

            _settingDialog.OnClickAsObservable().Subscribe(_=> SetActiveSettingDialog(false)).AddTo(gameObject);

            SetSettingDialog(userData);

			
		}
		private void SetSettingDialog(UserData userData)
		{
            SetSettingButton(SettingName.Sound, userData.onSound);
            SetSettingButton(SettingName.Vibration, userData.onVibration);
        }
        public void SetSettingButton(SettingName index, bool on)
        {
            _settingButtonImage[(int)index].sprite = on ? _settingOnSprite[(int)index] : _settingOffSprite[(int)index];
        }
		public void SetActiveSettingDialog(bool isActive)
		{
			
            var cd = _settingDialog.GetComponent<CanvasGroup>();

            if (isActive)
            {
                _settingDialog.gameObject.SetActive(true);
                _settingDialog.GetComponent<Fade>().FadeIn( cd,1.0f, _graphicRaycaster);
            }
            else
            {
                _settingDialog.GetComponent<Fade>().FadeOut(cd,1.0f, _graphicRaycaster);
            }
        }
	}
	public enum SettingName
	{
		Sound,
		Vibration,
		Rate,
	}
}