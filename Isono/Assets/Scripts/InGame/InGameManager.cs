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
        public void isClearChange(bool flag) { _isClear = flag; }

        [SerializeField] private GameObject _putObj = default;
        [SerializeField] private int _rimitObj = 3;
        [SerializeField] private int _strandLength = 2;
        [SerializeField] private Cube[] _stageObj = default;
        [SerializeField] private IngameView _ingameView = default;
        [SerializeField] public int connectCount = 0;
        public List<Cube> cubeList = default;
        private List<Cube> _cubeAllList = default;

        [SerializeField] private Transform  _linkStageCube;
        [SerializeField] private Transform  _linBlockCube;
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
            _ingameView.InitView(UserData.Instance.clearStage + 1);
            SetButtonEvent();

            mainCamera = Camera.main;

            _connecRanget.enabled = false;
            _connecRanget.transform.localScale = new Vector3(_connecRanget.transform.localScale.x * _strandLength, _connecRanget.transform.localScale.y * _strandLength, 1f); ;

            _cubeAllList = new List<Cube>();

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

            if ( cubeList == null )
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
                    var cubeObj    = Instantiate(cubePrefab, dataPos, Quaternion.identity, _linkStageCube);
                    cube           = cubeObj.GetComponent<StageCube>();
                }
                else
                {
                    cube.transform.localPosition = dataPos;
                    cube.transform.SetParent(_linkStageCube);
                }
                _cubeAllList.Add(cube);
                cubeList.Add(cube);
            }

            foreach (var dataPos in stageAsset.CubeBlockPosList)
            {
                var cube = getCacheCube<BlockCube>(_kKeyBlockCube);
                if (cube == null)
                {
                    var cubePrefab = ResourceManager.Load<GameObject>(_kPathBlockCubePrefab);
                    var cubeObj = Instantiate(cubePrefab, dataPos, Quaternion.identity, _linBlockCube);
                    cube = cubeObj.GetComponent<BlockCube>();
                }
                else
                {
                    cube.transform.localPosition = dataPos;
                    cube.transform.SetParent(_linBlockCube);
                }
                _cubeAllList.Add(cube);
            }

            _stageObj = cubeList.ToArray();
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

            foreach (var item in _stageObj)
            {
                cubeList.Add(item);
                _cubeObj.Add(item.gameObject);
                for (int i = 0; i < _stageObj.Length; i++)
                {
                    _stageObj[i].SetStatus(item);
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
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition + Camera.main.transform.forward);

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
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
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
                    var putObj = Instantiate(_putObj, offset, new Quaternion(), _linkStageCube);
                    cube       = putObj.GetComponent<PutCube>();
                }
                else
                {
                    cube.transform.localPosition = offset;
                    cube.transform.SetParent(_linkStageCube);
                }
                SetStrand(cube);
            }
        }

        private void SetStrand(PutCube putCube)
        {
            cubeList.Add(putCube);
            _cubeObj.Add(putCube.gameObject);
            _cubeAllList.Add(putCube);

            // PutCubeに各ブロック判定を追加
            foreach (var item in cubeList)
            {
                putCube.SetStatus(item);
            }

            putCube.InitLineRenderer(_stageObj.Length);

            for (int i = 0; i < cubeList.Count - 1; i++)
            {
                // 生成したPutCubeの判定を追加
                cubeList[i].SetStatus(putCube);
                if (Vector2.Distance(putCube.cubepos, cubeList[i].cubepos) < _strandLength)
                {
                    putCube.AddLineRendererObj(cubeList[i].cubepos);
                }
            }

            // 生成されているボックスの判定を更新
            for (int i = 0; i < cubeList.Count; i++)
            {
                for (int j = 0; j < putCube.connectFlag.Count; j++)
                {
                    if (putCube.connectFlag[i])
                    {
                        cubeList[i].connectFlag[j] = putCube.connectFlag[j];
                    }
                }
                // ここでマテリアル変えて
                //cubeList[i].MaterialChange();

                if (putCube.connectFlag[i] && i < _stageObj.Length)
                {
                    connectCount++;
                }
            }

            if (_stageObj.Length <= connectCount)
            {
                GameClear();
            }
            else
            {
                connectCount = 0;
            }
        }

        void GameClear()
        {
            _isClear = true;
            _ingameView.SetActiveClear(true);
            connectCount = 0;
        }

        private void SkinChange(int index)
        {
            UserData.Instance.SetMaterial(index);
            _ingameView.SetSelectButton(index);

            var connectSkinName = String.Format(_kSkin, index, 0);
            var putSkinName = String.Format(_kSkin, index, 2);

            for (int i = 0; i < _cubeObj.Count; i++)
            {
                switch (_cubeObj[i].tag)
                {
                    case ObjectTagInfo.STAGE_CUBE:
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

            cacheStage();

            _currentPutObj = 0;
            _stageObj = null;

            createStage(++_stageNum);
            UserData.Instance.SetClearStage(UserData.Instance.clearStage + 1);
            _ingameView.SetStageName(UserData.Instance.clearStage + 1);
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
                asset.Add(cubeTransform.localPosition);
            }

            foreach (Transform cubeTransform in _linBlockCube)
            {
                asset.AddBlock(cubeTransform.localPosition);
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

            foreach (var cubeTransform in asset.CubePosList)
            {
                var cubePrefab = AssetDatabase.LoadAssetAtPath(_kPathFullStageCubePrefab, typeof( GameObject )) as GameObject;
                var cubeObj    = PrefabUtility.InstantiatePrefab(cubePrefab, _linkStageCube.transform) as GameObject;
                cubeObj.transform.localPosition = cubeTransform;
            }

            foreach (var cubeTransform in asset.CubeBlockPosList)
            {
                var cubePrefab = AssetDatabase.LoadAssetAtPath(_kPathFullBlockCubePrefab, typeof(GameObject)) as GameObject;
                var cubeObj    = PrefabUtility.InstantiatePrefab(cubePrefab, _linBlockCube.transform) as GameObject;
                cubeObj.transform.localPosition = cubeTransform;
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