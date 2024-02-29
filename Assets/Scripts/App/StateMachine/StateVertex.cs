using System.Collections.Generic;
using App.StateMachine.BaseStates;

namespace App.StateMachine
{
    public class StateVertex
    {
        public VertexType VertexType => vertexType;

        protected VertexType vertexType = VertexType.State;
        
        protected List<StateTransition> Transitions;
        protected List<StateTransition> IncomingTransitions;

        protected IState state;

        public StateVertex(IState state)
        {
            this.state = state;
        }

        public void OnEnter()
        {
            state.OnEnter();
        }
        
        public void OnExit()
        {
            state.OnExit();
        }
        
        public StateTransition Event(IStateMachineEvent trigger)
        {
            return Transition().OnEvent(trigger);
        }
        
        public virtual StateTransition Transition()
        {
            var t = new StateTransition(this);

            if (Transitions == null)
                Transitions = new List<StateTransition> {t};
            else
                Transitions.Add(t);

            return t;
        }
        
        public virtual bool ExecuteTrigger(App.StateMachine.StateMachine stateMachine, IStateMachineEvent stateMachineEvent)
        {
            if (Transitions == null) return false;

            foreach (var transition in Transitions)
            {
                if (transition.ExecuteTrigger(stateMachine, stateMachineEvent))
                {
                    return true;
                }
            }

            return false;
        }

        public void AddIncomingTransition(StateTransition stateTransition)
        {
            if (IncomingTransitions == null)
                IncomingTransitions = new List<StateTransition> {stateTransition};
            else
                IncomingTransitions.Add(stateTransition);
        }
    }
}