using Santutu.Core.Extensions.Runtime.UnityExtensions;
using Santutu.Library.Shortcuts.Runtime;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace Santutu.Modules.UI.Runtime
{
    public class UIVisibleToggler : MonoBehaviour
    {
        [SerializeField] public ShortcutCollection shortcuts = new();

        [SerializeField] private CanvasGroup canvasGroup;


        private CanvasGroup CanvasGroup
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

        public void Show()
        {
            CanvasGroup.Show();
        }

        public void Hide()
        {
            CanvasGroup.Hide();
        }
#if ODIN_INSPECTOR  
        [Button("Toggle Visibility")]
#endif
        public void ToggleVisibility()
        {
            CanvasGroup.ToggleVisibility();
        }

        private void Update()
        {
            if (shortcuts.IsInvoked())
            {
                CanvasGroup.ToggleVisibility();
            }
        }
    }
}