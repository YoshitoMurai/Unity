using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.InGame
{
    public class StageManager : MonoBehaviour
    {
        public void Reset()
        {
            var linkCubeObj = GameObject.Find("Link_Cube");
            if(linkCubeObj != null)
            {
                _linkCube = linkCubeObj.GetComponent<Transform>();
            }
        }

        [SerializeField] Transform _linkCube;
        [SerializeField] int _stageNum;

#if UNITY_EDITOR
        [ContextMenu("SaveAsset")]
        void saveAsset()
        {
            var asset = StageDataSet.LoadForEditor(_stageNum);
            asset.Clear();

            var cubeTransformArray = _linkCube.GetComponentInChildren<Transform>();
            foreach (Transform cubeTransform in cubeTransformArray)
            {
                Debug.Log(cubeTransform.name);

                asset.Add(cubeTransform.localPosition);
            }

            asset.SetDirtyForEditor();
            Debug.Log(asset.name + "の生成に成功");

        }
#endif
    }
}