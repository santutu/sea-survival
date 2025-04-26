using Santutu.Core.Base.Runtime;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace Santutu.Modules.UI.Runtime.FloatingTexts
{
    public class FloatingDamageTextManager : SingletonMonoBehaviour<FloatingDamageTextManager>
    {
        [SerializeField] public GameObject prefab;


        public void Instantiate(int damage, Vector3 position)
        {
            var text = Instantiate(prefab, transform).GetComponent<FloatingText>();
            text.transform.position = position;
            text.transform.localRotation = Quaternion.identity;
            text.SetText(damage.ToString());
        }
    }
}