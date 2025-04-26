using Santutu.Core.Base.Runtime;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace Santutu.Modules.UI.Runtime
{
    public class WorldCanvas : SingletonMonoBehaviour<WorldCanvas>
    {
        private void LateUpdate()
        {
            transform.rotation = Camera.main.transform.rotation;
            
        }
    }
}