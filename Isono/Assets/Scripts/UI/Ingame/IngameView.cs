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

        // Start is called before the first frame update
        public void InitView(int stageNum)
        {
            _titleBackButton.OnClickAsObservable().Subscribe(_ => GameSceneManager.Instance.LoadScene(kSceneType.Title)).AddTo(gameObject);
            _backButton.OnClickAsObservable().Subscribe(_ => Debug.Log("やり直し")).AddTo(gameObject);
            _skinChangeButton.OnClickAsObservable().Subscribe(_ => _skinChangeDialog.SetActive(true)).AddTo(gameObject);
            _skinBackButton.OnClickAsObservable().Subscribe(_ => _skinChangeDialog.SetActive(false)).AddTo(gameObject);
            _unlockButton.OnClickAsObservable().Subscribe(_ => AdvertiseManager.Instance.ShowMovieAds());

            for (int i = 0; i < _skinButton.Length; i++)
            {
                var index = i;
                _skinButton[index].OnClickAsObservable().Subscribe(_ => Debug.Log(index));
            }
            _stageText.text = $"STAGE {1}";
        }
        public void SetStageName(int stageNum)
        {
            _stageText.text = $"STAGE {stageNum}";
        }
    }
}