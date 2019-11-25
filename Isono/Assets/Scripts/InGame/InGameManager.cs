using UniRx;
using UniRx.Triggers;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace IsonoGame.InGame
{
    public class InGameManager : MonoBehaviour
    {
        private Camera mainCamera;
        [SerializeField] private GameObject _putObj = default;
        [SerializeField] private int _rimitObj = 3;
        [SerializeField] private int _strandLength = 2;
        [SerializeField] private Cube[] _stageObj = default;
        private List<Cube> objectList = default;

        private int _currentPutObj = 0;
        void Start()
        {
            mainCamera = Camera.main;
            objectList = new List<Cube>();
            foreach (var item in _stageObj)
            {
                objectList.Add(item);
            }

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButton(0))
                .Subscribe(_ => KeepTouch())
                .AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonUp(0) && _currentPutObj < _rimitObj)
                .Subscribe(_ => ReleaseTouch())
                .AddTo(gameObject);
        }
        private void KeepTouch()
        {
            var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition + Camera.main.transform.forward);
        }
        private void ReleaseTouch()
        {
            var screenPoint = mainCamera.WorldToScreenPoint(transform.position);
            var offset = transform.position + Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            _currentPutObj++;

            SetStrand( Instantiate(_putObj, offset, new Quaternion()).GetComponent<PutCube>());
        }
        private void SetStrand(PutCube putCube)
        {
            putCube.InitLineRenderer();
            foreach (var cube in objectList)
            {
                if (Vector2.Distance(putCube.pos, cube.pos) < _strandLength)
                {
                    putCube.AddLineRendererObj(cube.pos);
                }
            }
        }
    }
}