using UnityEngine;

namespace sea_survival.Scripts.Stages
{
    public abstract class StageState : MonoBehaviour
    {
        protected StageSystem StageSystem => StageSystem.Ins;
        
        // 상태 진입시 호출
        public virtual void OnEnter()
        {
            gameObject.SetActive(true);
        }
        
        // 상태 종료시 호출
        public virtual void OnExit()
        {
            gameObject.SetActive(false);
        }
    }
} 