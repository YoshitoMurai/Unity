using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.InGame
{
    public class ProvisionalCube : Cube
    {
        [SerializeField] private LineRenderer lineRenderer = default;

        public void InitLineReset()
        {
            lineRenderer.positionCount = 0;
        }

        public void InitLineRemove(Cube removecube)
        {
            _instantiateCube.RemoveAll(s => s.transform.position == removecube.transform.position);
            //if(2 < lineRenderer.positionCount)
                lineRenderer.positionCount -= 2;
        }

        public void SetLineRendererCount(Cube cube)
        {
            SetStatus(cube);
            lineRenderer.positionCount += 2;
        }

        public void AddLineRenderer(Vector3 mousepos, int strandlength)
        {
            transform.position = mousepos;

            for (int i = 0; i < _instantiateCube.Count; i++)
            {
                lineRenderer.SetPosition(i * 2, transform.position);
                if (Vector2.Distance(transform.position, _instantiateCube[i].cubepos) < strandlength)
                {
                    lineRenderer.SetPosition((i * 2) + 1, _instantiateCube[i].cubepos);
                }
                else
                {
                    lineRenderer.SetPosition((i * 2) + 1, transform.position);
                }
            }
        }
    }
}
