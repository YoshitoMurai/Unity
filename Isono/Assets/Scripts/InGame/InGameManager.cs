﻿using UniRx;
using UniRx.Triggers;
using IsonoGame;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using Connect.InGame.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Connect.InGame
{
    public class InGameManager : MonoBehaviour
    {
        private const string _kPathCunnectCubePrefab = "Prefabs/InGame/ConnectCube";
        public void Reset()
        {
            var linkCubeObj = GameObject.Find("Link_Cube");
            if (linkCubeObj != null)
            {
                _linkCube = linkCubeObj.GetComponent<Transform>();
            }
        }
        private bool _isClear = false;

        [SerializeField] private GameObject _putObj       = default;
        [SerializeField] private int        _rimitObj     = 3;
        [SerializeField] private int        _strandLength = 2;
        [SerializeField] private Cube[]     _connectObj   = default;
        [SerializeField] private IngameView _ingameView   = default;
        [SerializeField] private int        _connectCount = 0;
        public List<Cube> putCubList = default;
        [SerializeField] private Transform  _linkCube;
        [SerializeField] private int        _stageNum;

        [SerializeField] public GameObject _connecRanget = default;

        private Camera mainCamera;
        private int _currentPutObj = 0;

        void Start()
        {
            _ingameView.InitView(1);
            _ingameView.OnClickClear
                .Subscribe(_ => NextStage())
                .AddTo(gameObject);
            mainCamera = Camera.main;
            putCubList = new List<Cube>();

            _connecRanget.SetActive(false);
            _connecRanget.transform.localScale = new Vector3(_connecRanget.transform.localScale.x * _strandLength, _connecRanget.transform.localScale.y * _strandLength, 1f); ;

            var stageAsset = StageDataSet.Load(_stageNum);
            foreach (var dataPos in stageAsset.CubePosList)
            {
                var connectPrefab = ResourceManager.Load<GameObject>(_kPathCunnectCubePrefab);
                var connectObj    = Instantiate(connectPrefab, dataPos, Quaternion.identity, _linkCube);
                var connect       = connectObj.GetComponent<Cube>();
                putCubList.Add(connect);
            }

            _connectObj = putCubList.ToArray();
            putCubList.Clear();

            foreach (var item in _connectObj)
            {
                putCubList.Add(item);
                for (int i = 0; i < _connectObj.Length; i++)
                {
                    _connectObj[i].SetConnect(item);
                }
            }

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButton(0) && !_isClear)
                .Subscribe(_ => KeepTouch())
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonUp(0) && _currentPutObj < _rimitObj && !UITouchOver() && !_isClear)
                .Subscribe(_ => ReleaseTouch())
                .AddTo(gameObject);
        }

        private bool UITouchOver()
        {
            //ボタンとかクリックされていたら無効にする
            if (EventSystem.current.currentSelectedGameObject != null) { return true; }

            //ImageとかTextでも被っていれば無効にする（マウスクリック）
            //if (EventSystem.current.IsPointerOverGameObject()) { return true; }
            //ImageとかTextでも被っていれば無効にする（タップ）
            //if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) { return true; }
            return false;
        }

        private void KeepTouch()
        {
            var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition + Camera.main.transform.forward);

            if (EventSystem.current.currentSelectedGameObject == null)
            {
                _connecRanget.transform.position = pos;
                _connecRanget.SetActive(true);
            }
        }

        private void ReleaseTouch()
        {
            _connecRanget.SetActive(false);

            var screenPoint = mainCamera.WorldToScreenPoint(transform.position);
            var offset = transform.position + Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            _currentPutObj++;

            SetStrand(Instantiate(_putObj, offset, new Quaternion()).GetComponent<PutCube>());
        }

        private void SetStrand(PutCube putCube)
        {
            foreach (var item in _connectObj)
            {
                putCube.SetConnect(item);
            }

            putCube.InitLineRenderer(_connectObj.Length);

            foreach (var putcubelist in putCubList)
            {
                if (Vector2.Distance(putCube.cubepos, putcubelist.cubepos) < _strandLength)
                {
                    putCube.AddLineRendererObj(putcubelist.cubepos);
                }
            }

            putCubList.Add(putCube);

            // つながっているオブジェクト判定を更新
            for (int i = 0; i < _connectObj.Length; i++)
            {
                for (int j = 0; j < putCube.connectFlag.Count; j++)
                {
                    if (putCube.connectFlag[i])
                    {
                        _connectObj[i].connectFlag[j] = putCube.connectFlag[j];
                    }
                }
                if (putCube.connectFlag[i]) _connectCount++;
            }

            GameClear();
        }

        void GameClear()
        {
            if (_connectObj.Length <= _connectCount)
            {
                _isClear = true;
                _ingameView.SetActiveClear(true);
                //yield return new WaitForSeconds(1f);
                //GameSceneManager.Instance.LoadScene(kSceneType.Title);
            }
            _connectCount = 0;
        }
        void NextStage()
        {
            _ingameView.SetActiveClear(false);
            _isClear = false;
        }

#if UNITY_EDITOR
        void saveAsset()
        {
            var asset = StageDataSet.LoadForEditor(_stageNum);
            asset.Clear();

            var cubeTransformArray = _linkCube.GetComponentInChildren<Transform>();
            foreach (Transform cubeTransform in cubeTransformArray)
            {
                asset.Add(cubeTransform.localPosition);
            }

            asset.SetDirtyForEditor();
            Debug.Log(asset.name + "の生成に成功");

        }

        [CustomEditor(typeof(InGameManager))]
        public class StageManagerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(30)))
                    {
                        var manager = target as InGameManager;
                        manager.saveAsset();
                    }
                }
            }
        }
#endif
    }
}