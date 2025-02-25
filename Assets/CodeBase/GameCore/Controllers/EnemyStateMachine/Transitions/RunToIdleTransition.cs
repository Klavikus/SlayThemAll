using CodeBase.GameCore.Controllers.StateMachines;
using CodeBase.GameCore.Domain.Enemies;

namespace CodeBase.GameCore.Controllers.EnemyStateMachine.Transitions
{
    class RunToIdleTransition : Transition
    {
        private readonly EnemyAI _enemyAI;

        public RunToIdleTransition(
            IEntityState nextEntityState,
            EnemyAI enemyAI)
            : base(nextEntityState)
        {
            _enemyAI = enemyAI;
            _enemyAI.StopMoving += MoveNextState;
        }

        public override void OnDispose() =>
            _enemyAI.StopMoving -= MoveNextState;
    }
}