using Santutu.Core.Base.Runtime;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace Santutu.Modules.UI.Runtime
{
    public class UICamera : SingletonMonoBehaviour<UICamera>
    {
        [SerializeField] private Camera value;

        public Camera Value
        {
            get
            {
                if (!value)
                {
                    value = GetComponent<Camera>();
                }

                return value;
            }
        }
    }
}