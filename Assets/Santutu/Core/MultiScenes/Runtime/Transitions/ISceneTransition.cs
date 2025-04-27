using Cysharp.Threading.Tasks;

namespace Santutu.Modules.MultiScenes.Runtime.Transitions
{
    public interface ISceneTransition 
    {
        UniTask In();
        UniTask Out();
        UniTask StartIn();
        UniTask StartOut();
    }
}