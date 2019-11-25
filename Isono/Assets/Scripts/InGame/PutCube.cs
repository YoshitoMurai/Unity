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
            lineRenderer.SetPosition(0, pos);
        }
        public void AddLineRendererObj(Vector3 pos)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount-1, pos);
        }
    }
}