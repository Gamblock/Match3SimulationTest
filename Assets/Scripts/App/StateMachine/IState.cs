namespace App.StateMachine
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
    }
}