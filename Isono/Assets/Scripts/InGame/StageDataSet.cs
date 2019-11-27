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
        const string _kAssetResourcePathFormat = "ScriptableObject/InGame/Stage_{0:000}";

        [SerializeField] List<Vector3> _cubePosList;

        public List<Vector3> CubePosList { get => _cubePosList; }

        /// <summary>
        /// ロード.
        /// </summary>
        /// <param name="stageNum"></param>
        /// <returns></returns>
        public static StageDataSet Load(int stageNum)
        {
            string path = string.Format(_kAssetResourcePathFormat, stageNum);
            var asset   = Resources.Load<StageDataSet>(path);
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
            var asset = new StageDataSet();
            AssetDatabase.CreateAsset(asset, path);

            return asset;
        }

        /// <summary>
        /// 更新反映.
        /// </summary>
        public void SetDirtyForEditor()
        {
            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// 追加.
        /// </summary>
        /// <param name="pos"></param>
        public void Add(Vector3 pos)
        {
            _cubePosList.Add(pos);
        }

        /// <summary>
        /// 初期化.
        /// </summary>
        public void Clear()
        {
            if (_cubePosList == null)
            {
                _cubePosList = new List<Vector3>();
                return;
            }
            _cubePosList.Clear();
        }
#endif
    }
}