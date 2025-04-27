using R3;
using UnityEngine;
using UnityEngine.Events;

namespace Santutu.Modules.MultiScenes.Runtime.Utils
{
    public class SceneLoadProgressChangedEventListener : MonoBehaviour
    {
        [SerializeField] public UnityEvent<float> onSceneLoadProgressChanged = new();

        private void Awake()
        {
            MultiSceneManager.OnSceneLoadProgressChanged.Subscribe(v => { onSceneLoadProgressChanged?.Invoke(v); })
                             .AddTo(this);
        }
    }
}