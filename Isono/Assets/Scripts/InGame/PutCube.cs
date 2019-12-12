using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Connect.InGame
{
    public class PutCube : Cube
    {
        [SerializeField] private LineRenderer lineRenderer = default;

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

        public void InitLineRenderer(int connectnumber)
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
            Debug.DrawLine(lineRenderer.GetPosition(0), putcubpos, Color.red, 3f);

            // Rayの衝突判定
            if (Physics.Linecast(lineRenderer.GetPosition(0), putcubpos, out hit))
            {
                //Rayが当たったオブジェクト判定
                switch (hit.collider.tag)
                {
                    case ObjectTagInfo.STAGE_CUBE:
                    case ObjectTagInfo.PUT_CUBE:
                        for (int i = 0; i < connectObj.Count - 1; i++)
                        {
                            if (connectObj[i].transform.position == putcubpos || 
                                hit.collider.gameObject.GetComponent<Cube>().connectFlag[i] && !connectFlag[i])
                            {
                                connectFlag[i] = true;
                            }
                        }
                        AddlineRenderer(putcubpos);
                        AddlineRenderer(cubepos);
                        break;

                    case ObjectTagInfo.BLOCK_CUBE: break;
                    default: break;
                }
            }
        }
    }
}