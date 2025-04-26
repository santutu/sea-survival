using Santutu.Core.Extensions.Runtime.UnityExtensions;
using UnityEngine;

namespace Santutu.Modules.UI.Runtime
{
    public class TooltipHandler : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public RectTransform target;

        public CanvasGroup CanvasGroup
        {
            get
            {
                if (canvasGroup == null)
                {
                    canvasGroup = this.GetOrAddComponent<CanvasGroup>();
                }

                return canvasGroup;
            }
        }

        private void Awake()
        {
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            Hide();
        }


        public void SetVisible(bool visible)
        {
            CanvasGroup.alpha = visible ? 1 : 0;
        }

        public void Show(RectTransform target)
        {
            ((RectTransform)transform).PositionNear(target);
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }
    }
}