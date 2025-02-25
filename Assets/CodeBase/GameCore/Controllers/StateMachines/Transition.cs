using System;

namespace CodeBase.GameCore.Controllers.StateMachines
{
    public abstract class Transition : IDisposable
    {
        private readonly IEntityState _nextEntityState;
     
        private EntityStateMachine _entityStateMachine;

        protected Transition(IEntityState nextEntityState) =>
            _nextEntityState = nextEntityState;

        public void BindContext(EntityStateMachine entityStateMachine) =>
            _entityStateMachine = entityStateMachine;

        public void Update() =>
            OnUpdate();

        public void MoveNextState() =>
            _entityStateMachine.ChangeState(_nextEntityState);

        public void Dispose() =>
            OnDispose();

        public virtual void OnUpdate()
        {
        }

        public virtual void OnDispose()
        {
        }
    }
}