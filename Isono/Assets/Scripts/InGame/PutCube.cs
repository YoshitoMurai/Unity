using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Connect.InGame
{
    public class PutCube : Cube
    {
        [SerializeField] private LineRenderer lineRenderer = default;
        private int _layerMask = 1 << 9;

        private void AddlineRenderer(Vector3 linepos)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, linepos);
        }

        public void InitLineReset()
        {
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, Vector3.zero);
        }

        public void InitLineRenderer()
        {
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.SetPosition(0, cubepos);
        }

        public void AddLineRendererObj(Vector3 putcubpos)
        {
            // Rayの作成
            Ray ray = new Ray(lineRenderer.GetPosition(0), putcubpos);

            //Rayの当たり判定
            RaycastHit hit;

            // Rayの可視化
            Debug.DrawLine(lineRenderer.GetPosition(0), putcubpos, Color.red, 4f);

            // Rayの衝突判定
            if (Physics.Linecast(lineRenderer.GetPosition(0), putcubpos, out hit, _layerMask))
            {
                var hitcube = hit.collider.gameObject.GetComponent<Cube>();

                //Rayが当たったオブジェクト判定
                if (hit.collider.tag != ObjectTagInfo.BLOCK_CUBE)
                {
                    for (int i = 0; i < _instantiateCube.Count - 1; i++)
                    {
                        if ((_instantiateCube[i].transform.position == putcubpos) ||
                            (hit.collider.GetComponent<Cube>().connectFlag[i] && !connectFlag[i]))
                        {
                            connectFlag[i] = true;

                        }
                    }
                    AddlineRenderer(putcubpos);
                    AddlineRenderer(cubepos);

                    switch (hit.collider.tag)
                    {
                        case ObjectTagInfo.STAGE_CUBE:
                            _stageCube.Add(hitcube);
                            break;

                        case ObjectTagInfo.PUT_CUBE:
                            foreach (var cube in hitcube._stageCube)
                            {
                                if (_stageCube.Contains(cube) == false)
                                {
                                    continue;
                                }

                                _stageCube.Add(cube);
                            }
                            break;

                        default: break;
                    }

                }
            }
        }
    }
}