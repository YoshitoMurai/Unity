using UniRx;
using UniRx.Triggers;
using IsonoGame;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Connect.InGame.UI;
using Connect.Common;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Connect.InGame
{
    public class InGameManager : MonoBehaviour
    {
        private const string _kSkin = "Materials/Skin/Skin{0}/M_Skin{0}_{1}";
        private const string _kPathCunnectCubePrefab = "Prefabs/InGame/ConnectCube";

        private const int _kKeyPutCube   = 0;
        private const int _kKeyStageCube = 1;

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
        public List<Cube> cubeList = default;

        [SerializeField] private Transform  _linkCube;
        [SerializeField] private Transform  _cacheCube;
        [SerializeField] private int        _stageNum;

        Color red = new Color(1.0f, 0.3f, 0.3f, 0.3f);
        Color white = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        public Image _connecRanget = default;

        [SerializeField] private List<GameObject> _cubeObj = default;

        private Camera mainCamera;
        private int _currentPutObj = 0;
        private Dictionary<int, List<Cube>> _cacheCubeDict = new Dictionary<int, List<Cube>>();
        private SkinManager skinManager;


        void Start()
        {
            // Debug 毎回1ステージ目から
            UserData.Instance.SetClearStage(0);

            skinManager = new SkinManager();
            skinManager.LoadSkinData();

            _ingameView.InitView(UserData.Instance.clearStage+1);
            _ingameView.OnClickClear
                .ThrottleFirst(TimeSpan.FromSeconds(1))
                .Subscribe(_ => NextStage())
                .AddTo(gameObject);

            _ingameView.OnClickUnlock.Subscribe(_ =>
            {
                AdvertiseManager.Instance.ShowMovieAds();
                _ingameView.SetUnsealedButton(skinManager.GetRandomSkinId());
                });
            mainCamera = Camera.main;

            _connecRanget.enabled = false;
            _connecRanget.transform.localScale = new Vector3(_connecRanget.transform.localScale.x * _strandLength, _connecRanget.transform.localScale.y * _strandLength, 1f); ;

            // ステージ生成.
            createStage(_stageNum);

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButton(0) && !_isClear)
                .Subscribe(_ => KeepTouch())
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonUp(0) && !UITouchOver() && !_isClear)
                .Subscribe(_ => ReleaseTouch())
                .AddTo(gameObject);

            SkinChange(UserData.Instance.selectMaterial);

            // スキン変更
            _ingameView.OnClickSkin
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(index =>
                {
                    UserData.Instance.SetMaterial(index);
                    SkinChange(index);
                }).AddTo(gameObject);
        }

        private void createStage(int stageNum)
        {
            var stageAsset = StageDataSet.Load(stageNum);

            if (stageAsset == null)
            {
                Debug.LogError("存在しないステージを生成しようとしています");
                return;
            }

            if( cubeList == null )
            {
                cubeList = new List<Cube>();
            }
            cubeList.Clear();

            foreach (var dataPos in stageAsset.CubePosList)
            {
                var cube = getCacheCube<StageCube>(_kKeyStageCube);
                if (cube == null)
                {
                    var cubePrefab = ResourceManager.Load<GameObject>(_kPathCunnectCubePrefab);
                    var cubeObj    = Instantiate(cubePrefab, dataPos, Quaternion.identity, _linkCube);
                    cube           = cubeObj.GetComponent<StageCube>();
                }
                else
                {
                    cube.transform.localPosition = dataPos;
                    cube.transform.SetParent(_linkCube);
                }
                cubeList.Add(cube);
            }

            _connectObj = cubeList.ToArray();
            cubeList.Clear();
            _cubeObj.Clear();
            _cubeObj.Add(_putObj.gameObject);
            _cubeObj.Add(ResourceManager.Load<GameObject>(_kPathCunnectCubePrefab));

            foreach (Transform child in _cacheCube)
            {
                if (child.tag != ObjectTagInfo.TAG_DEFAULT)
                {
                    _cubeObj.Add(child.gameObject);
                }
            }

            foreach (var item in _connectObj)
            {
                cubeList.Add(item);
                _cubeObj.Add(item.gameObject);
                for (int i = 0; i < _connectObj.Length; i++)
                {
                    _connectObj[i].SetConnect(item);
                }
            }
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
            Ray ray        = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            var pos        = mainCamera.ScreenToWorldPoint(Input.mousePosition + Camera.main.transform.forward);

            if (EventSystem.current.currentSelectedGameObject == null)
            {
                _connecRanget.transform.position = Input.mousePosition;
                _connecRanget.enabled = true;
            }

            // オブジェクトが生成できる場合とできないでUIの色を変える
            if (Physics.BoxCast(ray.origin, new Vector3(0.6f, 0.6f, 0.6f), ray.direction, out hit))
            {
                _connecRanget.color = red;
            }
            else
            {
                _connecRanget.color = white;
            }
        }

        private void ReleaseTouch()
        {
            Ray ray        = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            _connecRanget.enabled = false;

            // オブジェクトがあった場合は、生成させない
            if (!Physics.BoxCast(ray.origin, new Vector3(0.6f, 0.6f, 0.6f), ray.direction, out hit) && _currentPutObj < _rimitObj)
            {
                var screenPoint = mainCamera.WorldToScreenPoint(transform.position);
                var offset = transform.position + mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                _currentPutObj++;

                PutCube cube = getCacheCube<PutCube>(_kKeyPutCube);
                if (cube == null)
                {
                    var putObj = Instantiate(_putObj, offset, new Quaternion(), _linkCube);
                    cube       = putObj.GetComponent<PutCube>();
                }
                else
                {
                    cube.transform.localPosition = offset;
                    cube.transform.SetParent(_linkCube);
                }
                SetStrand(cube);
            }
        }

        private void SetStrand(PutCube putCube)
        {
            foreach (var item in _connectObj)
            {
                putCube.SetConnect(item);
            }

            putCube.InitLineRenderer(_connectObj.Length);

            foreach (var putcubelist in cubeList)
            {
                if (Vector2.Distance(putCube.cubepos, putcubelist.cubepos) < _strandLength)
                {
                    putCube.AddLineRendererObj(putcubelist.cubepos);
                }
            }

            List<int> connectCheck = new List<int>();
            for(int k = 0; k < putCube.connectFlag.Count; k++)
            {
                var putFlag = putCube.connectFlag[k];
                if (putFlag)
                {
                    connectCheck.Add(k);
                }
            }

            // つながっているオブジェクト判定を更新
            if (connectCheck.Count != _connectObj.Length)
            {
                for (int i = 0; i < cubeList.Count; i++)
                {
                    var cubelist = cubeList[i];
                    
                    for (int j = 0; j < connectCheck.Count; j++)
                    {
                        var check = connectCheck[j];
                        switch (cubelist.gameObject.tag)
                        {
                            case ObjectTagInfo.CONNECT_CUBE:
                                if (putCube.connectFlag[i])
                                {
                                    cubelist.connectFlag[check] = putCube.connectFlag[check];
                                }
                                break;
                            case ObjectTagInfo.PUT_CUBE:
                                // どちらかがtrueだった場合に判定を更新する
                                if(cubelist.connectFlag[check] && putCube.connectFlag[check])
                                {
                                    foreach(var update in connectCheck)
                                    {
                                        cubelist.connectFlag[update] = putCube.connectFlag[update];
                                    }
                                    j++;
                                }
                                break;
                            default: break;
                        }
                    }
                }
            }
            else
            {
                GameClear();
            }

            cubeList.Add(putCube);
            _cubeObj.Add(putCube.gameObject);
        }

        void GameClear()
        {
            _isClear = true;
            _ingameView.SetActiveClear(true);
        }

        private void SkinChange(int index)
        {
            UserData.Instance.SetMaterial(index);
            _ingameView.SetSelectButton(index);

            var connectSkinName = String.Format(_kSkin, index, 0);
            var putSkinName     = String.Format(_kSkin, index, 2);

            for (int i = 0; i < _cubeObj.Count; i++)
            {
                switch (_cubeObj[i].tag)
                {
                    case ObjectTagInfo.CONNECT_CUBE:
                        _cubeObj[i].GetComponent<Renderer>().material = skinManager.skins[index, 0];
                        break;

                    case ObjectTagInfo.PUT_CUBE:
                        _cubeObj[i].GetComponent<Renderer>().material = skinManager.skins[index, 2];
                        break;

                    default: Debug.Log("スキンがねぇな"); break;
                }
            }
        }

        private void NextStage()
        {
            _ingameView.SetActiveClear(false);

            _isClear = false;

            cacheStarg();

            _currentPutObj = 0;
            _connectObj = null;

            createStage(++_stageNum);
            UserData.Instance.SetClearStage(UserData.Instance.clearStage+1);
            _ingameView.SetStageName(UserData.Instance.clearStage+1);
        }

        private void cacheStarg()
        {
            foreach (var cube in cubeList)
            {
                int index;
                // タッチで生成するオブジェクト.
                switch (cube)
                {
                    case PutCube putCube:     index = _kKeyPutCube;   break;
                    case StageCube StageCube: index = _kKeyStageCube; break;
                    default:
                        Debug.Log("未定義のCubeがあります");
                        continue;
                }

                List<Cube> cacheList;
                if (_cacheCubeDict.TryGetValue(index, out cacheList) == false)
                {
                    cacheList = new List<Cube>();
                    _cacheCubeDict.Add(index, cacheList);
                }

                cacheList.Add(cube);
                cube.gameObject.SetActive(false);
                cube.transform.SetParent(_cacheCube);
                cube.Clear();

                if (cube.gameObject.tag == ObjectTagInfo.PUT_CUBE)
                {
                    var lineCube = cube.GetComponent<PutCube>();
                    lineCube.InitLineReset();
                }
            }
        }

        private T getCacheCube<T>(int key) where T : Cube
        {
            T cube = null;
            List<Cube> list;
            if (_cacheCubeDict.TryGetValue(key, out list))
            {
                cube = list[0] as T;
                cube.gameObject.SetActive(true);
                list.RemoveAt(0);
                if (list.Count == 0)
                {
                    _cacheCubeDict.Remove(key);
                }
            }
            return cube;
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