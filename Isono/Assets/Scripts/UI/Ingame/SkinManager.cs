using UniRx;
using UnityEngine;
using System.Linq;
using Connect.Common;

namespace Connect.InGame
{
	public class SkinManager 
	{

        private const string _kPathSkin = "Materials/Skin/Skin";
        private const string _kPathCunnectCubePrefab = "Prefabs/InGame/ConnectCube";

        public Material[,] skins { get; private set; }

		int kSkinMax = 12;
		int kSkinType = 3;
		public void LoadSkinData()
		{
			skins = new Material[kSkinMax, kSkinType];
			
			for (int id = 0; id < kSkinMax; id++)
			{
				for (int type = 0; type < kSkinType; type++)
				{
					skins[id, type] = Resources.Load(_kPathSkin + id + "/" + "M_Skin" + id + "_" + type) as Material;
				}
			}
		}

		public int GetRandomSkinId()
		{
			// 解放されてないスキンのID取得
			var sealedSkinIds = UserData.Instance.isUnsealedSkin.Select((unsealed, index) => new { Unsealed = unsealed, Index = index })
								.Where(skin => !skin.Unsealed)
								.Select(skin => skin.Index).ToArray();
            if (sealedSkinIds.Length == 0) return -1;
			var unsealedId = Random.Range(0, sealedSkinIds.Length);

			return sealedSkinIds[unsealedId];
		}
	}
}