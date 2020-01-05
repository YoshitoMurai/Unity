using UniRx;
using UniRx.Triggers;
using IsonoGame;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
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
        private const string _kPathFullStageCubePrefab = "Assets/Resources/" + _kPathCunnectCubePrefab + ".prefab";
        private const string _kPathCunnectCubePrefab   = "Prefabs/InGame/StageCube";

        private const string _kPathFullBlockCubePrefab = "Assets/Resources/" + _kPathBlockCubePrefab + ".prefab";
        private const string _kPathBlockCubePrefab     = "Prefabs/InGame/BlockCube";

        private const int _kKeyPutCube   = 0;
        private const int _kKeyStageCube = 1;
        private const int _kKeyBlockCube = 2;

        public void Reset()
        {
            var linkCubeObj = GameObject.Find("Link_Cube");
            if (linkCubeObj != null)
            {
                _linkStageCube = linkCubeObj.GetComponent<Transform>();
            }
        }

        private bool _isClear = false;

        [SerializeField] private GameObject _putObj = default;
        [SerializeField] private int _rimitObj = 3;
        [SerializeField] private int _strandLength = 2;
        [SerializeField] private Cube[] _stageObj = default;
        [SerializeField] private IngameView _ingameView = default;
        [SerializeField] private int connectCount = 0;
        public List<Cube> connectCubeList = default;
        private List<Cube> _cubeAllList = default;

        // 追加
        [SerializeField] private List<int> _connectColor = default;
        [SerializeField] private List<int> _colorNum = default;
        //public List<int> colorChuck = new List<int>();

        [SerializeField] private Transform  _linkStageCube = default;
        [SerializeField] private Transform  _linBlockCube = default;
        [SerializeField] private Transform  _cacheCube = default;
        [SerializeField] private int        _stageNum;
        [SerializeField] private bool       _stageDebug;

        [SerializeField] private ProvisionalCube _provisionalCube = default;

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
            int stageNum = _stageDebug ? _stageNum : UserData.Instance.nextStage;
            _ingameView.InitView(stageNum);
            _ingameView.OnClickBack.Subscribe(_ =>
            {
                RetryStage();
            });
            SetButtonEvent();

            mainCamera = Camera.main;
            _provisionalCube.gameObject.SetActive(false);
            _cubeAllList = new List<Cube>();

            // ステージ生成.
            createStage(stageNum);

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButton(0) && !_isClear)
                .Subscribe(_ => KeepTouch())
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonUp(0) && !UITouchOver() && !_isClear)
                .Subscribe(_ => ReleaseTouch())
                .AddTo(gameObject);

            _ingameView.OnClickSkinChange.Subscribe(_ =>
            {
                _isClear = true;
                _ingameView.ChangeSkinUI(_isClear);
            });

            _ingameView.OnClickSkinBack.Subscribe(_ =>
            {
                _isClear = false;
                _ingameView.ChangeSkinUI(_isClear);
            });

            SetSkin(UserData.Instance.selectSkin);

            // スキン変更
            _ingameView.OnClickSkin
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(index =>
                {
                    UserData.Instance.SetSkin(index);
                    SetSkin(index, false);
                }).AddTo(gameObject);
        }
        private void SetButtonEvent()
        {
            _ingameView.OnClickClear
                .ThrottleFirst(TimeSpan.FromSeconds(1))
                .Subscribe(_ => NextStage())
                .AddTo(gameObject);

            _ingameView.OnClickUnlock.Subscribe(_ =>
            {
                AdvertiseManager.Instance.ShowMovieAds();
                _ingameView.SetUnsealedButton(skinManager.GetRandomSkinId());
            });
            _ingameView.OnClickBack.Subscribe(_ => Debug.Log("やりなおし"));
        }
        private void createStage(int stageNum)
        {
            var stageAsset = StageDataSet.Load(stageNum);

            if (stageAsset == null)
            {
                Debug.LogError("存在しないステージを生成しようとしています");
                return;
            }

            _cubeAllList.Clear();

            if ( connectCubeList == null )
            {
                connectCubeList = new List<Cube>();
            }
            connectCubeList.Clear();

            foreach (var data in stageAsset.cubeStageDataList)
            {
                var cube = getCacheCube<StageCube>(_kKeyStageCube);
                if (cube == null)
                {
                    var cubePrefab = ResourceManager.Load<GameObject>(_kPathCunnectCubePrefab);
                    var cubeObj    = Instantiate(cubePrefab, data.pos, Quaternion.identity, _linkStageCube);
                    cube           = cubeObj.GetComponent<StageCube>();
                }
                else
                {
                    cube.transform.localPosition = data.pos;
                    cube.transform.SetParent(_linkStageCube);
                }
                cube._colorNumber = data.colorNum;
                _cubeAllList.Add(cube);
                cube.InitMaterial(skinManager.skins[UserData.Instance.selectSkin]);
                SetColor(cube);
                cube.SetRotate(false);
                connectCubeList.Add(cube);
            }

            foreach (var data in stageAsset.cubeBlockDataList)
            {
                var cube = getCacheCube<BlockCube>(_kKeyBlockCube);
                if (cube == null)
                {
                    var cubePrefab = ResourceManager.Load<GameObject>(_kPathBlockCubePrefab);
                    var cubeObj = Instantiate(cubePrefab, data.pos, Quaternion.identity, _linBlockCube);
                    cube = cubeObj.GetComponent<BlockCube>();
                }
                else
                {
                    cube.transform.localPosition = data.pos;
                    cube.transform.SetParent(_linBlockCube);
                }
                cube._colorNumber = data.colorNum;
                cube.InitMaterial(skinManager.skins[UserData.Instance.selectSkin]);
                cube.SetColor(skinManager.GetSkinColor(SkinColorType.Block), SkinColorType.Block);
                _cubeAllList.Add(cube);
            }

            _stageObj = connectCubeList.ToArray();
            connectCubeList.Clear();
            _ingameView.SetCurrentPutObjText(_rimitObj - _currentPutObj);

            connectCount = _stageObj.Length;
            _connectColor.Clear();

            int max = 0;

            foreach (var item in _stageObj)
            {
                connectCubeList.Add(item);
                _provisionalCube.SetLineRendererCount(item);
                for (int i = 0; i < _stageObj.Length; i++)
                {
                    _stageObj[i].SetStatus(item);
                }

                if (max < item._colorNumber + 1)
                {
                    max = item._colorNumber + 1;
                }
            }

            for(int i = 0; i < max; i++)
            {
                _connectColor.Add(0);
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
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition + Camera.main.transform.forward);

            if (EventSystem.current.currentSelectedGameObject == null)
            {
                pos = new Vector3(pos.x, pos.y, 0f);
                _provisionalCube.gameObject.SetActive(true);
                _provisionalCube.AddLineRenderer(pos, _strandLength);
            }

            // オブジェクトが生成できる時とできない時でUIの色を変える
            if (Physics.BoxCast(ray.origin, new Vector3(0.6f, 0.6f, 0.6f), ray.direction, out hit))
            {
                _provisionalCube.SetColor(skinManager.GetSkinColor(SkinColorType.Block), SkinColorType.Block);
            }
            else
            {
                _provisionalCube.SetColor(skinManager.GetSkinColor(SkinColorType.UnConnect), SkinColorType.UnConnect);
            }
        }

        private void ReleaseTouch()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            _provisionalCube.gameObject.SetActive(false);

            // オブジェクトがあった場合は、生成させない
            if (!Physics.BoxCast(ray.origin, new Vector3(0.6f, 0.6f, 0.6f), ray.direction, out hit) && _currentPutObj < _rimitObj)
            {
                var screenPoint = mainCamera.WorldToScreenPoint(transform.position);
                var offset = transform.position + mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                _currentPutObj++;
                _ingameView.SetCurrentPutObjText(_rimitObj - _currentPutObj);

                PutCube cube = getCacheCube<PutCube>(_kKeyPutCube);
                if (cube == null)
                {
                    var putObj = Instantiate(_putObj, offset, new Quaternion(), _linkStageCube);
                    cube       = putObj.GetComponent<PutCube>();
                }
                else
                {
                    cube.transform.localPosition = offset;
                    cube.transform.SetParent(_linkStageCube);
                }
                cube.InitMaterial(skinManager.skins[UserData.Instance.selectSkin]);
                cube.SetColor(skinManager.GetSkinColor(SkinColorType.Connect), SkinColorType.Connect);
                cube.transform.rotation = Quaternion.identity;
                SetStrand(cube);
            }
        }

        private void SetStrand(PutCube putCube)
        {
            connectCubeList.Add(putCube);
            _cubeAllList.Add(putCube);
            _provisionalCube.SetLineRendererCount(putCube);

            // PutCubeに各ブロック判定を追加
            foreach (var item in connectCubeList)
            {
                putCube.SetStatus(item);
            }

            putCube.InitLineRenderer();

            for (int i = 0; i < connectCubeList.Count - 1; i++)
            {
                // 生成したPutCubeの判定を追加
                connectCubeList[i].SetStatus(putCube);
                if (Vector2.Distance(putCube.cubepos, connectCubeList[i].cubepos) < _strandLength)
                {
                    putCube.AddLineRendererObj(connectCubeList[i].cubepos);
                }
            }

            putCube.ChangeLayer(9);

            // 生成されているボックスの判定を更新
            for (int i = 0; i < connectCubeList.Count; i++)
            {
                var cube = connectCubeList[i];
                for (int j = 0; j < putCube.connectFlag.Count; j++)
                {
                    if (putCube.connectFlag[i])
                    {
                        cube.connectFlag[j] = putCube.connectFlag[j];

                        // つながっているStageCubeを格納
                        if (cube._stageCube.Count == 0)
                        {
                            foreach(var stagecube in putCube._stageCube)
                            {
                                cube._stageCube.Add(stagecube);
                            }
                        }
                    }
                }
                //if (putCube.connectFlag[i] && i < _stageObj.Length)
                //{
                //    cube.InitMaterial(skinManager.skins[UserData.Instance.selectSkin]);
                //    cube.SetColor(skinManager.GetSkinColor(SkinColorType.Connect), SkinColorType.Connect);
                //    cube.SetRotate(true);
                //}
            }

            // 追加
            if (1 < putCube._stageCube.Count)
            {
                List<int> chuckColor = new List<int>();
                List<int> changeColor = new List<int>();
                _colorNum.Clear();

                // Cubeの色が一致しているかを調べる
                for (int a = 0; a < putCube._stageCube.Count; a++)
                {
                    _connectColor[putCube._stageCube[a]._colorNumber]++;
                }

                for (int y = 0; y < _connectColor.Count; y++)
                {
                    if (1 == _connectColor[y])
                    {
                        changeColor.Add(y);
                    }
                }

                for (int t = 0; t < changeColor.Count; t++)
                {
                        if (t == 0)
                        {
                            _colorNum.Add(changeColor[changeColor.Count - 1]);
                        }
                        else if (t != changeColor.Count)
                        {
                            _colorNum.Add(changeColor[t - 1]);
                        }
                }

                for (int b = 0; b < _connectColor.Count; b++)
                {
                    if (1 < _connectColor[b])
                    {
                        chuckColor.Add(b);
                    }
                    else if (_connectColor.Count - 1 <= b)
                    {
                        for (int i = 0; i < putCube._stageCube.Count; i++)
                        {
                            if (_colorNum[i] != 99)
                            {
                                putCube._stageCube[i]._colorNumber = _colorNum[i];
                                SetColor(putCube._stageCube[i]);
                            }
                        }
                        Debug.Log("AAA");
                    }
                    _connectColor[b] = 0;
                }

                // 色が一致したCubeを非表示にする予定
                if (chuckColor.Count != 0)
                {
                    for (int j = 0; j < putCube._instantiateCube.Count; j++)
                    {
                        if (putCube.connectFlag[j] && putCube._instantiateCube[j].gameObject.activeSelf)
                        {
                            switch (putCube._instantiateCube[j].tag)
                            {
                                case ObjectTagInfo.STAGE_CUBE:
                                    foreach (var color in chuckColor)
                                    {
                                        if (color == putCube._instantiateCube[j]._colorNumber)
                                        {
                                            putCube._instantiateCube[j].transform.SetParent(_cacheCube);
                                            putCube._instantiateCube[j].gameObject.SetActive(false);
                                            _provisionalCube.InitLineRemove(putCube._instantiateCube[j]);
                                            connectCount--;
                                        }
                                    }
                                    break;
                                case ObjectTagInfo.PUT_CUBE:
                                    putCube._instantiateCube[j].transform.SetParent(_cacheCube);
                                    putCube._instantiateCube[j].gameObject.SetActive(false);
                                    _provisionalCube.InitLineRemove(putCube._instantiateCube[j]);
                                    break;
                                default: break;
                            }
                        }
                    }
                    //colorChuck.Clear();
                }
            }

            if (connectCount == 0)
            {
                GameClear();
            }
            else
            {
                //connectFlag.Clear();
            }
        }

        void GameClear()
        {
            _isClear = true;
            _ingameView.SetActiveClear(true);
            //connectColor.Clear();
        }

        private void SetColor(Cube cube)
        {
            switch (cube._colorNumber)
            {
                case 0: cube.SetColor(skinManager.GetSkinColor(SkinColorType.Connect), SkinColorType.Connect); break;
                case 1: cube.SetColor(skinManager.GetSkinColor(SkinColorType.UnConnect), SkinColorType.UnConnect); break;
                case 2: cube.SetColor(skinManager.GetSkinColor(SkinColorType.Block), SkinColorType.Block); break;
                case 3: cube.SetColor(skinManager.GetSkinColor(SkinColorType.green), SkinColorType.green); break;
                case 4: cube.SetColor(skinManager.GetSkinColor(SkinColorType.pink), SkinColorType.pink); break;
                default: break;
            }
        }

        private void SetSkin(int index) { SetSkin(index, true); }
        private void SetSkin(int index, bool resetColor)
        {
            UserData.Instance.SetSkin(index);
            _ingameView.SetSelectButton(index);

            for (int i = 0; i < _cubeAllList.Count; i++)
            {
                var cube = _cubeAllList[i].GetComponent<Cube>();
                cube.SetSkin(skinManager.skins[index]);
                if (resetColor)
                {
                    switch (cube.tag)
                    {
                        case ObjectTagInfo.STAGE_CUBE:
                            SetColor(cube);
                            break;
                        case ObjectTagInfo.PUT_CUBE:
                            cube.SetColor(skinManager.GetSkinColor(SkinColorType.Connect), SkinColorType.Connect);
                            break;
                        case ObjectTagInfo.BLOCK_CUBE:
                            cube.SetColor(skinManager.GetSkinColor(SkinColorType.Block), SkinColorType.Block);
                            break;
                        default: Debug.Log("スキンがねぇな"); break;
                    }
                }
            }
            _provisionalCube.SetSkin(skinManager.skins[index]);
            _provisionalCube.SetColor(skinManager.GetSkinColor(SkinColorType.UnConnect), SkinColorType.UnConnect);
        }

        private void NextStage()
        {
            _ingameView.SetActiveClear(false);

            _isClear = false;

            cacheStage();

            _currentPutObj = 0;
            _stageObj = null;

            UserData.Instance.SetClearStage(UserData.Instance.nextStage);
            _ingameView.SetStageName(UserData.Instance.nextStage);

            createStage(UserData.Instance.nextStage);
        }

        private void RetryStage()
        {
            if(_isClear)
            {
                return;
            }

            cacheStage();

            _currentPutObj = 0;
            _stageObj = null;

            createStage(UserData.Instance.nextStage);
        }

        private void cacheStage()
        {
            foreach (var cube in _cubeAllList)
            {
                int index;
                // タッチで生成するオブジェクト.
                switch (cube)
                {
                    case PutCube putCube: index = _kKeyPutCube; break;
                    case StageCube StageCube: index = _kKeyStageCube; break;
                    case BlockCube blockCube: index = _kKeyBlockCube; break;
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
            _provisionalCube.InitLineReset();
            _provisionalCube.Clear();

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

            foreach (Transform cubeTransform in _linkStageCube)
            {
                var cube = cubeTransform.GetComponent<Cube>();
                asset.Add(new StageData(cube._colorNumber, cube.transform.localPosition));
            }

            foreach (Transform cubeTransform in _linBlockCube)
            {
                var cube = cubeTransform.GetComponent<Cube>();
                asset.AddBlock(new StageData(cube._colorNumber, cube.transform.localPosition));
            }

            asset.SetDirtyForEditor();
            Debug.Log(asset.name + "の生成に成功");

        }

        void loadAsset()
        {
            var asset = StageDataSet.LoadForEditor(_stageNum);
            if( asset == null )
            {
                Debug.LogError("アセットが読み込めません");
                return;
            }

            foreach (var cubeData in asset.cubeStageDataList)
            {
                var cubePrefab = AssetDatabase.LoadAssetAtPath(_kPathFullStageCubePrefab, typeof( GameObject )) as GameObject;
                var cubeObj    = PrefabUtility.InstantiatePrefab(cubePrefab, _linkStageCube.transform) as GameObject;
                var cube       = cubeObj.GetComponent<Cube>();

                cube._colorNumber = cubeData.colorNum;
                cube.transform.localPosition = cubeData.pos;
            }

            foreach (var cubeData in asset.cubeBlockDataList)
            {
                var cubePrefab = AssetDatabase.LoadAssetAtPath(_kPathFullBlockCubePrefab, typeof(GameObject)) as GameObject;
                var cubeObj    = PrefabUtility.InstantiatePrefab(cubePrefab, _linBlockCube.transform) as GameObject;
                var cube       = cubeObj.GetComponent<Cube>();

                cube._colorNumber = cubeData.colorNum;
                cube.transform.localPosition = cubeData.pos;
            }
        }

        void deleteGameObject()
        {
            for (int ii = _linkStageCube.childCount - 1; 0 <= ii; ii--)
            {
                var obj = _linkStageCube.GetChild(ii);
                DestroyImmediate(obj.gameObject);
            }

            for (int ii = _linBlockCube.childCount - 1; 0 <= ii; ii--)
            {
                var obj = _linBlockCube.GetChild(ii);
                DestroyImmediate(obj.gameObject);
            }
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

                    if (GUILayout.Button("Load", GUILayout.Width(100), GUILayout.Height(30)))
                    {
                        var manager = target as InGameManager;
                        manager.deleteGameObject();
                        manager.loadAsset();
                    }

                    if (GUILayout.Button("Destory", GUILayout.Width(100), GUILayout.Height(30)))
                    {
                        var manager = target as InGameManager;
                        manager.deleteGameObject();
                    }
                }
            }
        }
#endif
    }
}