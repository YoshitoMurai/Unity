using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Connect.InGame
{
    public class Cube : MonoBehaviour
    {
        public Vector3 cubepos => new Vector3(transform.position.x, transform.position.y, transform.position.z);
        public List<Cube> _instantiateCube;
        public List<bool> connectFlag;
        public Material cubeMaterial = default;
        [SerializeField] private Rotate rotate = default;
        private Material mat;
        bool init = false;

        public List<Cube> _stageCube;
        public int _colorNumber;

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
        public void SetColor(Color color, SkinColorType colornumber)
        {
            mat.SetColor("_Color", color);
            //_colorNumber = (int)colornumber;

            // デバッグ用
            //_colorNumber = Random.Range(0, 2);
            _colorNumber = 1;

        }
        public void SetRotate(bool isRotate)
        {
            if (rotate != null) rotate.isRotate = isRotate;
            if (!isRotate) transform.rotation = Quaternion.identity;
        }

        public void SetStatus(Cube cube)
        {
            _instantiateCube.Add(cube);

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
            _stageCube.Clear();
            _instantiateCube.Clear();
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