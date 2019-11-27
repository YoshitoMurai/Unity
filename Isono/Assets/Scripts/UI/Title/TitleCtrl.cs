using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEngine.SceneManagement;
using IsonoGame;

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
                    _model.SwitchActiveSound();
                    _view.SetSettingButton(settingName, _model.userData.setting.onSound);
                    break;
                case SettingName.Vibration:
                    _model.SwitchActiveVib();
                    _view.SetSettingButton(settingName, _model.userData.setting.onVibration);
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
            GameSceneManager.Instance.LoadScene(GameSceneManager.kSceneType.Game);
        }
    }
}