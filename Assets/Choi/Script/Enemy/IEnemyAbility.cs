using UnityEngine;

namespace Choi
{
    public interface IEnemyAbility
    {
        void Setup(Enemy_Main enemy); // Enemy 참조 전달
        void OnEnter();          // 상태 시작 (외형 활성화 등)
        void OnTick();           // 매 프레임 로직 (타이머, 공격 등)
        void OnExit();           // 상태 종료 (외형 비활성화 등)
    }
}
