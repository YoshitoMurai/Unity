using UniRx;
using UnityEngine;
using System.Linq;
using Connect.Common;

namespace Connect.InGame
{
    public enum SkinColorType
    {
        Connect,
        UnConnect,
        Block,

        // デバック用
        green,
        pink,
    }
	public class SkinManager 
	{

        private const string _kPathSkin = "Shaders/Skin/S_Skin";
        private const string _kPathCunnectCubePrefab = "Prefabs/InGame/ConnectCube";

        public Shader[] skins { get; private set; }

		int kSkinMax = 12;
        [SerializeField] private Color _connectColor = new Color ( 1.3f, 0.87f, 0.0f, 1.0f );
        [SerializeField] private Color _unconnectColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        [SerializeField] private Color _blockColor = new Color(1.0f, 0.0f, 0.25f, 1.0f);

        // デバック用
        [SerializeField] private Color _green = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        [SerializeField] private Color _pink = new Color(0.0f, 1.0f, 0.75f, 1.0f);

        private Color _errorColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
        public void LoadSkinData()
		{
			skins = new Shader[kSkinMax];
			
			for (int id = 0; id < kSkinMax; id++)
			{
				skins[id] = Resources.Load(_kPathSkin  + id) as Shader;
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
        public Color GetSkinColor(SkinColorType colorType)
        {
            switch (colorType)
            {
                case SkinColorType.Connect:return _connectColor;
                case SkinColorType.UnConnect:return _unconnectColor;
                case SkinColorType.Block:return _blockColor;

                // デバック用
                case SkinColorType.green: return _green;
                case SkinColorType.pink: return _pink;
            }
            return _errorColor;
        }
	}
}