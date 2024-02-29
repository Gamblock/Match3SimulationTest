using System;
using Core.StateMachine;

namespace App.StateMachine
{
    public class StateTransition
    {
        public StateVertex SourceStateVertex;
        public StateVertex TargetStateVertex;
        public IStateMachineEvent TriggerEvent;

        public StateTransition(StateVertex source)
        {
            SourceStateVertex = source;
        }

        public StateTransition Target(StateVertex target)
        {
            if (TargetStateVertex != null) throw new Exception("Duplicated transition target");
            TargetStateVertex = target;
            TargetStateVertex.AddIncomingTransition(this);
            return this;
        }


        public bool ExecuteTrigger(App.StateMachine.StateMachine stateMachine, IStateMachineEvent stateMachineEvent)
        {
            if (TriggerEvent == stateMachineEvent)
            {
                stateMachine.PrepareTransition(TargetStateVertex);
                return true;
            }

            return false;
        }

        public StateTransition OnEvent(IStateMachineEvent trigger)
        {
            TriggerEvent = trigger;
            return this;
        }
    }
}