using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
namespace Connect.InGame.UI
{
    public class SkinButton : MonoBehaviour
    {
        Color unsealedColor = new Color(0.15f, 0.15f, 0.15f, 0.5f);
        Color selectColor = new Color(0f, 0f, 0f, 0f);
        Color sealedColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        [SerializeField] private Image image = default;
        [SerializeField] private Button button = default;
        private bool unsealed = true;
        private int id = 0;
        public IObservable<int> OnClick => button.OnClickAsObservable().Where(_=> unsealed).Select(index => id);
        public void Initialize(int index, bool unsealed, bool select)
        {
            this.unsealed = unsealed;
            id = index;
            if (unsealed)
            {
                if (select)
                {
                    image.color = selectColor;
                }
                else
                {
                    image.color = unsealedColor;
                }
            }
            else
            {
                
                image.color = sealedColor;
            }
        }
        public void SetSelect(bool active)
        {
            image.color = active ? selectColor : unsealedColor;
        }
        public void SetUnsealed()
        {
            unsealed = false;
            image.color = unsealedColor;
        }
    }
}