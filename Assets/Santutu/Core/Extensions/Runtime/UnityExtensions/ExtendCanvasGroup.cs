using System.Threading;
using Cysharp.Threading.Tasks;
using Santutu.Core.Extensions.Runtime.Enums;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendCanvasGroup
    {
        public static void Show(this CanvasGroup canvasGroup)
        {
            canvasGroup.SetAlphaToOne();
            canvasGroup.EnableInteraction();
        }

        public static void Hide(this CanvasGroup canvasGroup)
        {
            canvasGroup.SetAlphaToZero();
            canvasGroup.DisableInteraction();
        }

        public static void SetAlphaToOne(this CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1;
        }

        public static void SetAlphaToZero(this CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0;
        }

        public static void EnableInteraction(this CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }


        public static void DisableInteraction(this CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }


        public static bool IsHidden(this CanvasGroup canvasGroup)
        {
            return canvasGroup.alpha <= 0 && canvasGroup.interactable == false && canvasGroup.blocksRaycasts == false;
        }

        public static bool IsVisible(this CanvasGroup canvasGroup)
        {
            return canvasGroup.alpha >= 1 && canvasGroup.interactable && canvasGroup.blocksRaycasts;
        }


        public static void SetVisible(this CanvasGroup canvasGroup, bool visible)
        {
            if (visible)
            {
                canvasGroup.Show();
            }
            else
            {
                canvasGroup.Hide();
            }
        }

        public static ShowOrHide ToggleVisibility(this CanvasGroup canvasGroup)
        {
            if (canvasGroup.IsVisible())
            {
                canvasGroup.Hide();
                return ShowOrHide.Hide;
            }

            canvasGroup.Show();
            return ShowOrHide.Show;
        }
    }
}