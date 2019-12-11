using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using TMPro;
using Connect;
using Connect.Common;

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
        [SerializeField] private SkinButton[] _skinButton = default;
        public IObservable<Unit> OnClickUnlock => _unlockButton.OnClickAsObservable();
        private int selectSkin;

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
            
            selectSkin = UserData.Instance.selectMaterial;
            for (int i = 0; i < _skinButton.Length; i++)
            {
                _skinButton[i].Initialize(i,UserData.Instance.isUnsealedSkin[i], selectSkin == i);
            }
            OnClickSkin = _skinButton[0].OnClick.Merge(
                _skinButton[1].OnClick,
                _skinButton[2].OnClick,
                _skinButton[3].OnClick,
                _skinButton[4].OnClick,
                _skinButton[5].OnClick,
                _skinButton[6].OnClick,
                _skinButton[7].OnClick,
                _skinButton[8].OnClick,
                _skinButton[9].OnClick,
                _skinButton[10].OnClick,
                _skinButton[11].OnClick
                );
            

            _stageText.text = $"STAGE {stageNum}";

            _clearbg.GetComponent<CanvasGroup>().alpha = 0;
        }
        public void SetStageName(int stageNum)
        {
            Debug.Log(stageNum);
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
        public void SetSelectButton(int after)
        {
            _skinButton[selectSkin].SetSelect(false);
            _skinButton[after].SetSelect(true);
            selectSkin = after;
        }
        public void SetUnsealedButton(int index)
        {
            _skinButton[index].SetUnsealed();
        }
    }
}