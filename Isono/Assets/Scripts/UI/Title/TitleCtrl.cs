using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEngine.SceneManagement;
using IsonoGame;
using Connect;
using Connect.Common;

namespace Connect.Title
{
    public class TitleCtrl : MonoBehaviour
    {
        [SerializeField] private TitleView _view = default;
        [SerializeField] private AudioSource _bgm = default;
        void Start()
        {
            _bgm.mute = !UserData.Instance.onSound;
            _view.OpenView(UserData.Instance);
            _view.onClickStart
                .Subscribe(_ => GameStart())
                .AddTo(gameObject);
            
            _view.onClickSetting
                .Subscribe(_ => _view.SetActiveSettingDialog(true))
                .AddTo(gameObject);

            _view.onClickNoAds
                .Subscribe(_ => _view.SetActiveSettingDialog(true))
                .AddTo(gameObject);

            _view.onClickInstagram
                .Subscribe(_ => Application.OpenURL("https://www.instagram.com/baya.sea/"))
                .AddTo(gameObject);
            
            _view.onClickTwitter
                .Subscribe(_ => Application.OpenURL("https://twitter.com/7373Homa"))
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
                    UserData.Instance.SwitchSound();
                    _view.SetSettingButton(settingName, UserData.Instance.onSound);
                    _bgm.mute = !UserData.Instance.onSound;
                    break;
                case SettingName.Vibration:
                    UserData.Instance.SwitchVib();
                    _view.SetSettingButton(settingName, UserData.Instance.onVibration);
                    break;
                case SettingName.Rate:
                    Debug.Log("Rate");
                    //_model.SwitchActiveReview();
                    //_view.SetSettingButton(settingName, _model.userData.setting.alreadyReview);
                    break;
            }
        }
        private void GameStart()
        {
            GameSceneManager.Instance.LoadScene(kSceneType.Game);
        }
    }
}