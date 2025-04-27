#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Santutu.Core.Base.Runtime.Utils
{
    public class ApplicationExitButton : MonoBehaviour
    {
        [SerializeField] private Button btn;

        private void Awake()
        {
            if (btn == null)
            {
                btn = GetComponent<Button>();
            }
        }

        private void Start()
        {
            btn.onClick.AddListener(HandleExitButtonClick);
        }

        private void OnDestroy()
        {
            btn.onClick.RemoveListener(HandleExitButtonClick);
        }

        private void HandleExitButtonClick()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
        }
    }
}