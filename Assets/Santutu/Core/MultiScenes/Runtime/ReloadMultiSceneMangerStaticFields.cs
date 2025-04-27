using UnityEngine;

namespace Santutu.Modules.MultiScenes.Runtime
{
    public static class ReloadMultiSceneMangerStaticFields
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            MultiSceneManager.UseForceStartScene = false;
            MultiSceneManager.ForceStartScene = null;
        }
    }
}