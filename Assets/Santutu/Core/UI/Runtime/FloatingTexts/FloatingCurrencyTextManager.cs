using Santutu.Core.Base.Runtime;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace Santutu.Modules.UI.Runtime.FloatingTexts
{
    public class FloatingCurrencyTextManager : SingletonMonoBehaviour<FloatingCurrencyTextManager>
    {
        [SerializeField] public GameObject prefab;


        public void Instantiate(string text, Vector3 position)
        {
            var floatingText = Instantiate(prefab, transform).GetComponent<FloatingText>();
            floatingText.transform.position = position;
            floatingText.transform.localRotation = Quaternion.identity;
            floatingText.SetText(text);
        }
    }
}