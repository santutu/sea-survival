using System.Threading;
using Cysharp.Threading.Tasks;
using Santutu.Core.Extensions.Runtime.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Santutu.Core.Tween.Runtime.Enums;
using Santutu.Core.Tween.Runtime.Extensions;

namespace Santutu.Modules.UI.Runtime
{
    public class HoverTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text text;

        [SerializeField] private Color color = Color.white;

        private Color _originalColor;

        [SerializeField] private float inDuration = 0.25f;
        [SerializeField] private float outDuration = 0.25f;

        private CancellationTokenSource _cts;

        private void Awake()
        {
            _originalColor = text.color;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            TaskHelper.RefreshToken(ref _cts);
            text.TwColor(color, duration: inDuration, TimeScale.Unscaled, _cts.Token).Forget();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TaskHelper.RefreshToken(ref _cts);
            text.TwColor(_originalColor, duration: outDuration, TimeScale.Unscaled, _cts.Token).Forget();
        }
    }
}