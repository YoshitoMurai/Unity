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

        public void SetLineRendererCount(Cube cube)
        {
            SetStatus(cube);
            lineRenderer.positionCount++;
            lineRenderer.positionCount++;
        }

        public void AddLineRenderer(Vector3 mousepos, int strandlength)
        {
            transform.position = mousepos;

            for (int i = 0; i < connectObj.Count; i++)
            {
                lineRenderer.SetPosition(i * 2, transform.position);
                if (Vector2.Distance(transform.position, connectObj[i].cubepos) < strandlength)
                {
                    lineRenderer.SetPosition((i * 2) + 1, connectObj[i].cubepos);
                }
                else
                {
                    lineRenderer.SetPosition((i * 2) + 1, transform.position);

                }
            }
        }
    }
}
