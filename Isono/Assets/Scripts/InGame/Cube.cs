using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Connect.InGame
{
    public class Cube : MonoBehaviour
    {
        public Vector3 cubepos => new Vector3(transform.position.x, transform.position.y, transform.position.z);
        [SerializeField] public List<bool> connectFlag;
        [SerializeField] public List<Cube> connectObj = default;

        public void MaterialChange()
        {

        }

        public void SetStatus(Cube cube)
        {
            connectObj.Add(cube);

            if (cubepos != cube.transform.position)
            {
                connectFlag.Add(false);
            }
            else if (cubepos == cube.transform.position)
            {
                connectFlag.Add(true);
            }
        }

        public void Clear()
        {
            connectObj.Clear();
            connectFlag.Clear();
        }
    }
}