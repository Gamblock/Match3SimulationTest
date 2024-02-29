using App.StateMachine.BaseStates;
using Core.StateMachine;

namespace App.StateMachine
{
    public class StateMachineEditor
    {
        private StateMachine stateMachine;

        public StateMachineEditor(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public StateVertex Initial()
        {
            var initialVertex = new InitialVertex(null);
            stateMachine.SetInitialVertex(initialVertex);
            return initialVertex;
        }

        public StateVertex Final()
        {
            var finalVertex = new FinalVertex(null);
            AddVertex(finalVertex);
            return finalVertex;
        }

        private void AddVertex(StateVertex vertex)
        {
            stateMachine.AddVertex(vertex);
        }

        public void Destroy()
        {
            stateMachine = null;
        }
    }
}