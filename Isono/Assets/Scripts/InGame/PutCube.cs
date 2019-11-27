using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IsonoGame.InGame
{
    public class PutCube : Cube
    {
        [SerializeField] private LineRenderer lineRenderer = default;

        public void InitLineRenderer()
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
            Debug.DrawLine(lineRenderer.GetPosition(0), putcubpos, Color.red, 1f);

            // Rayの衝突判定
            if (Physics.Linecast(lineRenderer.GetPosition(0), putcubpos, out hit))
            {
                //Rayが当たったオブジェクトのtagがBlockCubeだったら
                switch (hit.collider.tag)
                {
                    case "PutCube":
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