
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Isono.StageSelect
{
	public class StageSelectCtrl : MonoBehaviour
	{
		private StageSelectModel _model = default;
		[SerializeField] private StageSelectView _view = default;
		void Start()
		{
			_model = new StageSelectModel();
			_view.OpenView(_model.userData);

			_view.onClickBack
				.Subscribe(_ => SceneManager.LoadScene("Title"))
				.AddTo(gameObject);
			/*
			_view.onClickLabel
				.Subscribe(id => Debug.Log(id))
				.AddTo(gameObject);*/
		}
	}
}