using IsonoGame;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Connect.InGame
{
    public class StageDataSet : ScriptableObject
    {
        const string _kAssetFullPathFormat     = "Assets/Resources/" + _kAssetResourcePathFormat;
        const string _kAssetResourcePathFormat = "ScriptableObject/InGame/Stage_{0:000}.asset";

        [SerializeField] List<StageData> _cubeStageDataList;
        [SerializeField] List<StageData> _cubeBlockDataList = default;

        public List<StageData> cubeStageDataList { get => _cubeStageDataList; }
        public List<StageData> cubeBlockDataList { get => _cubeBlockDataList; }

        /// <summary>
        /// ロード.
        /// </summary>
        /// <param name="stageNum"></param>
        /// <returns></returns>
        public static StageDataSet Load(int stageNum)
        {
            string path = string.Format(_kAssetResourcePathFormat, stageNum);
            var asset   = ResourceManager.Load<StageDataSet>(path);
            Debug.Assert(asset != null, "StageDataSetが取得できません Path: " + path);

            return asset;
        }

#if UNITY_EDITOR

        /// <summary>
        /// エディタ用ロード.
        /// </summary>
        /// <param name="stageNum"></param>
        /// <returns></returns>
        public static StageDataSet LoadForEditor(int stageNum)
        {
            string path = string.Format(_kAssetFullPathFormat, stageNum);
            var asset = AssetDatabase.LoadAssetAtPath<StageDataSet>(path);
            if (asset == null)
            {
                asset = CreateForEditor(path);
            }

            return asset;
        }

        /// <summary>
        /// エディタ用生成.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static StageDataSet CreateForEditor(string path)
        {
            var asset = CreateInstance<StageDataSet>();
            AssetDatabase.CreateAsset(asset, path);

            return asset;
        }

        /// <summary>
        /// 更新反映.
        /// </summary>
        public void SetDirtyForEditor()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 追加.
        /// </summary>
        /// <param name="data"></param>
        public void Add(StageData data)
        {
            _cubeStageDataList.Add(data);
        }

        /// <summary>
        /// ギミック追加.
        /// </summary>
        /// <param name="data"></param>
        public void AddBlock(StageData data)
        {
            _cubeBlockDataList.Add(data);
        }

        /// <summary>
        /// 初期化.
        /// </summary>
        public void Clear()
        {
            if (_cubeStageDataList == null)
            {
                _cubeStageDataList = new List<StageData>();
                _cubeBlockDataList = new List<StageData>();
                return;
            }
            _cubeStageDataList.Clear();
            _cubeBlockDataList.Clear();
        }
#endif
    }

    [System.Serializable]
    public class StageData
    {
        [SerializeField] private int _colorNum;
        [SerializeField] private Vector3 _pos;

        public int colorNum { get => _colorNum; }
        public Vector3 pos { get => _pos; }

        public StageData(int colorNum, Vector3 pos)
        {
            _colorNum = colorNum;
            _pos      = pos;
        }
    }
}