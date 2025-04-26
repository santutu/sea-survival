using System.Threading;
using Santutu.Core.Base.Runtime.Singletons;

namespace Santutu.Core.Base.Runtime.Helpers
{
    public class SceneMono : AutoInstantiateSingletonMonoBehaviour<SceneMono>
    {
        public static CancellationToken DestroyCancellationToken => Instance.destroyCancellationToken;
    }
}