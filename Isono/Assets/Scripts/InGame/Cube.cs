using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Connect.InGame
{
    public class Cube : MonoBehaviour
    {
        public Vector2 cubepos => new Vector2(transform.position.x, transform.position.y);
        [SerializeField] public List<bool> connectFlag;
        [SerializeField] public List<Cube> connectObj = default;

        public void MaterialChange()
        {

        }

        public void SetStatus(Cube cube, bool flag)
        {
            connectObj.Add(cube);
            connectFlag.Add(flag);
        }

        public void Clear()
        {
            connectObj.Clear();
            connectFlag.Clear();
        }
    }
}