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

    }
}