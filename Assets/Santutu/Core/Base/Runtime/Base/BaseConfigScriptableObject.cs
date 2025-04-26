using Santutu.Core.Base.Runtime.Constants;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace Santutu.Core.Base.Runtime.Base
{
    [Icon(Paths.EditorIconPath + "/Config Icon.png")]
    public abstract class BaseConfigScriptableObject<T> : SingletonScriptableObject<T> where T : BaseConfigScriptableObject<T>
    {
    }
}