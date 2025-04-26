using Santutu.Core.Base.Runtime;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace Santutu.Modules.UI.Runtime
{
    public class UICanvas : SingletonMonoBehaviour<UICanvas>
    {
        [SerializeField]
        public Canvas value;
        public Canvas Value
        {
            get
            {
                if (value == null)
                {
                    value = GetComponent<Canvas>();
                }

                return value;
            }
        }
    }
}