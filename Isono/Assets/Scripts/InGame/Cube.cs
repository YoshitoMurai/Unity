using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Connect.InGame
{
    public class Cube : MonoBehaviour
    {
        public Vector3 cubepos => new Vector3(transform.position.x, transform.position.y, transform.position.z);
        public List<bool> connectFlag;
        public List<Cube> connectObj = default;
        public Material cubeMaterial = default;
        [SerializeField] private Rotate rotate = default;
        private Material mat;
        bool init = false;

        public void ChangeLayer(int layer)
        {
            gameObject.layer = layer;
        }

        public void InitMaterial(Shader shader)
        {
            mat = GetComponent<Renderer>().material = new Material(shader);
            init = true;
        }
        public void SetSkin(Shader shader)
        {
            if (!init) InitMaterial(shader);
            mat.shader = shader;
        }
        public void SetColor(Color color)
        {
            mat.SetColor("_Color", color);
        }
        public void SetRotate(bool isRotate)
        {
            if (rotate != null) rotate.isRotate = isRotate;
            if (!isRotate) transform.rotation = Quaternion.identity;
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
            switch (gameObject.tag)
            {
                case ObjectTagInfo.STAGE_CUBE: break;
                case ObjectTagInfo.PUT_CUBE  : ChangeLayer(0); break;
                case ObjectTagInfo.BLOCK_CUBE: break;
                default: break;
            }
        }

        public void OnDestroy()
        {
            Destroy(mat);
        }
    }
}