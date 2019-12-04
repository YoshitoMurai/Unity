using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Connect.InGame
{
    public class PutCube : Cube
    {
        [SerializeField] private LineRenderer lineRenderer = default;

        public void InitLineRenderer(int connectnumber)
        {
            lineRenderer.SetWidth(0.05f, 0.05f);
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
                    case "ConnectObj":
                        for (int i = 0; i < connectObj.Count; i++)
                        {
                            if (connectObj[i].transform.position == putcubpos)
                            {
                                connectFlag[i] = true;
                            }
                            else if(hit.collider.gameObject.GetComponent<Cube>().connectFlag[i] && !connectFlag[i])
                            {
                                connectFlag[i] = true;
                            }
                        }

                        lineRenderer.positionCount++;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, putcubpos);

                        lineRenderer.positionCount++;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, cubepos);
                        break;

                    case "PutCube":
                        for (int j = 0; j < connectObj.Count; j++)
                        {
                            if (hit.collider.gameObject.GetComponent<Cube>().connectFlag[j] && !connectFlag[j])
                            {
                                connectFlag[j] = true;
                            }
                        }

                        lineRenderer.positionCount++;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, putcubpos);

                        lineRenderer.positionCount++;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, cubepos);
                        break;

                    case "BlockCube":
                        break;

                    default: break;
                }
            }
        }
    }
}