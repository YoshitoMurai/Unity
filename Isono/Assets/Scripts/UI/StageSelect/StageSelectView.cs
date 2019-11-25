
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UniRx.Triggers;
using UnityEngine.UI;
namespace Isono.StageSelect
{
	public class StageSelectView : MonoBehaviour
	{
		[SerializeField] private Button _backButton = default;
		[SerializeField] private StageSelectLabel[] stageSelectLabels = default;
		public IObservable<Unit> onClickBack => _backButton.onClick.AsObservable();
		public IObservable<int> onClickLabel;
		public void OpenView(UserData userData)
		{
			/*
			for (int i = 0; i < userData.clearStage+1; i++)
			{
				stageSelectLabels[i].Initialize(i,userData.starNum[i]);
			}
			onClickLabel = stageSelectLabels.Select(value => value.OnClick).Merge();
			*/
			// 広告表示



			// UniRx について
			/*
			// Return : 引数のデータを処理に入れる
			Observable.Return(new Vector2(0, 1))
				.Subscribe(v => Move(v.x,v.y))
				.AddTo(gameObject);

			// Where : 条件を満たしたときのみ処理を実行
			this.UpdateAsObservable()
				.Where(_=>Input.GetMouseButton(0))
				.Subscribe(_ => Move(0.01f,0))
				.AddTo(gameObject);

			// Select : 処理に対して引数を与えることができる
			this.UpdateAsObservable()
				.Select(_ => -0.5f)
				.Subscribe(l => Move(0.01f * l, 0))
				.AddTo(gameObject);

			// First : 一度だけ処理を実行
			this.UpdateAsObservable()
				.First(_ => Input.GetMouseButton(1))
				.Subscribe(_ => Move(0, -1))
				.AddTo(gameObject);

			// クリック時テキスト変更
			OnClick.SubscribeToText(_text,_ => (int.Parse(_text.text) + 1).ToString()).AddTo(gameObject);

			//

			// SkipUntil : 登録されているイベントが起きるまで処理をSkip
			// TakeUntil : 登録されているイベントが起きたら処理を止める
			// Repeat : 再度Subscribeする。
			this.UpdateAsObservable()
				.SkipUntil(_buttonOn.ClickObservable)
				.Select(_ => new Vector2(Input.GetAxis("Mouse X")*0.01f, Input.GetAxis("Mouse Y")*0.01f))
				.TakeUntil(_buttonOff.ClickObservable)
				.Repeat()
				.Subscribe(move => Move(move.x, move.y))
				.AddTo(gameObject);


			// DistinctUntilChanged : 値が変化した時のみ処理を通す
			/*this.UpdateAsObservable()
				.Select(_ => flag)
				.DistinctUntilChanged()
				.Where(x => x)
				.Subscribe(_ => Debug.Log("AAA"))
				.AddTo(gameObject);
			*/
		}

	}
}