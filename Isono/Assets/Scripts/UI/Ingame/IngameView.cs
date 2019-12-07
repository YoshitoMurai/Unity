using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using TMPro;
using Connect;

namespace Connect.InGame.UI
{
    public class IngameView : MonoBehaviour
    {
        [SerializeField] private Button _titleBackButton = default;
        [SerializeField] private Button _backButton = default;
        [SerializeField] private Button _skinChangeButton = default;
        [SerializeField] private TextMeshProUGUI _stageText = default;
        [SerializeField] private GameObject _skinChangeDialog = default;

        [Header("SkinChange")]
        [SerializeField] private Button _skinBackButton = default;
        [SerializeField] private Button _unlockButton = default;
        [SerializeField] private Button[] _skinButton = default;

        [Header("Clear")]
        [SerializeField] private Button _clearbg = default;
        public IObservable<Unit> OnClickClear => _clearbg.OnClickAsObservable();

        public IObservable<int> OnClickSkin;

        [SerializeField] private GraphicRaycaster _graphicRaycaster = default;

        // Start is called before the first frame update
        public void InitView(int stageNum)
        {
            _titleBackButton.OnClickAsObservable().Subscribe(_ => GameSceneManager.Instance.LoadScene(kSceneType.Title)).AddTo(gameObject);
            _backButton.OnClickAsObservable().Subscribe(_ => Debug.Log("やり直し")).AddTo(gameObject);
            _skinChangeButton.OnClickAsObservable().Subscribe(_ => _skinChangeDialog.SetActive(true)).AddTo(gameObject);
            _skinBackButton.OnClickAsObservable().Subscribe(_ => _skinChangeDialog.SetActive(false)).AddTo(gameObject);
            _unlockButton.OnClickAsObservable().Subscribe(_ => AdvertiseManager.Instance.ShowMovieAds());
            
            OnClickSkin = _skinButton[0].OnClickAsObservable().Select(_=>0).Merge(
                _skinButton[1].OnClickAsObservable().Select(_ => 1),
                _skinButton[2].OnClickAsObservable().Select(_ => 2),
                _skinButton[3].OnClickAsObservable().Select(_ => 3),
                _skinButton[4].OnClickAsObservable().Select(_ => 4),
                _skinButton[5].OnClickAsObservable().Select(_ => 5),
                _skinButton[6].OnClickAsObservable().Select(_ => 6),
                _skinButton[7].OnClickAsObservable().Select(_ => 7),
                _skinButton[8].OnClickAsObservable().Select(_ => 8),
                _skinButton[9].OnClickAsObservable().Select(_ => 9),
                _skinButton[10].OnClickAsObservable().Select(_ => 10),
                _skinButton[11].OnClickAsObservable().Select(_ => 11)
                );
            

            _stageText.text = $"STAGE {stageNum}";

            _clearbg.GetComponent<CanvasGroup>().alpha = 0;
        }
        public void SetStageName(int stageNum)
        {
            _stageText.text = $"STAGE {stageNum}";
        }
        public void SetActiveClear(bool isActive)
        {
            var cd = _clearbg.GetComponent<CanvasGroup>();

            if (isActive)
            {
                _clearbg.gameObject.SetActive(true);
                _clearbg.GetComponent<Fade>().FadeIn(cd, 1.0f, _graphicRaycaster);
            }
            else
            {
                _clearbg.GetComponent<Fade>().FadeOut(cd, 1.0f, _graphicRaycaster);
            }
        }
    }
}