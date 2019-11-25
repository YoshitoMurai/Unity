using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
namespace Isono.StageSelect
{
	public class StageSelectLabel : MonoBehaviour
	{
		[SerializeField] private Button _button = default;
		[SerializeField] private Image[] _starImage = default;
		[SerializeField] private Sprite _starSprite = default;
		private int _id = -1;
		public IObservable<int> OnClick => _button.OnClickAsObservable().Select(_ => _id);
		public void Initialize(int id, int starNum)
		{
			_button.interactable = true;
			_id = id;
			for (int i = 0; i < starNum; i++)
			{
				_starImage[i].sprite = _starSprite;
			}
		}
	}
}
