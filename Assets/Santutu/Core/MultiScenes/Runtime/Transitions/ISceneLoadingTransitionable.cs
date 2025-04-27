using Cysharp.Threading.Tasks;

namespace Santutu.Modules.MultiScenes.Runtime.Transitions
{
    public interface ISceneLoadingTransitionable
    {
        public  UniTask<ISceneTransition> LoadSceneAndGetTransition();

        public  UniTask UnloadScene();
    }
}