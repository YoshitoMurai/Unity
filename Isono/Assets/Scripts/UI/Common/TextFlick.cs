using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;
using TMPro;

namespace Isono.Common.UI
{
	public class TextFlick : MonoBehaviour
	{
		[SerializeField] private float _speed = 0.1f;
		[SerializeField, Range(0f, 1f)] private float _minAlpha = 0.5f;
		internal bool isFlick = true;
		void Start()
		{
			if (gameObject.GetComponent<TextMeshProUGUI>())
			{
				var image = gameObject.GetComponent<TextMeshProUGUI>();
				Observable
					.Interval(TimeSpan.FromSeconds(0.01f))
					.Where(_ => isFlick)
					.Subscribe(time => image.color = UpdateColor(image.color, time))
					.AddTo(gameObject);

			}
		}
		private Color UpdateColor(Color color, float time)
		{
			color.a = ((Mathf.Sin(time * _speed) * 0.5f + 0.5f) * _minAlpha) + (1 - _minAlpha);

			return color;
		}
	}
}