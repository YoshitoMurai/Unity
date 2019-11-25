using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Isono.StageSelect
{
	public class StageSelectModel
	{
		public UserData userData { get; private set; }

		public StageSelectModel()
		{
			userData = new UserData();
		}
	}
}