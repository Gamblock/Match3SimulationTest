namespace App.StateMachine.BaseStates
{
    public class FinalVertex : StateVertex
    {
        public FinalVertex(IState state) : base(state)
        {
            vertexType = VertexType.Final;
        }
    }
}