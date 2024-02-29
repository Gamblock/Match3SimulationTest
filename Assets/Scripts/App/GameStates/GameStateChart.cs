using System;
using App.GameStates.States;
using App.StateMachine;
using Core.StateMachine;
using Features.Signals;
using JetBrains.Annotations;
using Zenject;

namespace App.GameStates
{
    [UsedImplicitly]
    public class GameStateEvents
    {
        public static readonly IStateMachineEvent StartLevelEvent = new StateMachineEvent();
        public static readonly IStateMachineEvent ExitBoardEvent = new StateMachineEvent();
        public static readonly IStateMachineEvent DisposedEvent = new StateMachineEvent();
    }
    
    [UsedImplicitly]
    public class GameStateChart : IInitializable, IDisposable
    {
        [Inject] 
        private DiContainer diContainer;
        
        [Inject] 
        private SignalBus signalBus;

        App.StateMachine.StateMachine stateMachine;
        
        public void Initialize()
        {
            stateMachine = new App.StateMachine.StateMachine();
            SetupStateChart(stateMachine);
            stateMachine.Start();
            
            signalBus.Subscribe<ChangeLevelSignal>(OnLevelChange);
            signalBus.Subscribe<ExitToMapSignal>(OnExitToMap);
        }

        private void OnExitToMap()
        {
            stateMachine.Trigger(GameStateEvents.ExitBoardEvent);
        }

        private void OnLevelChange()
        {
            stateMachine.Trigger(GameStateEvents.StartLevelEvent);
        }

        private void SetupStateChart(App.StateMachine.StateMachine machine)
        {
            var bootState = new StateVertex(diContainer.Instantiate<BootState>());
            var mapState = new StateVertex(diContainer.Instantiate<BoardConfigSelectionState>());
            var boardState = new StateVertex(diContainer.Instantiate<BoardState>());
            
            var editor = new StateMachineEditor(machine);
            editor.Initial().Transition().Target(mapState);
            bootState.Transition().Target(mapState);
            mapState.Event(GameStateEvents.StartLevelEvent).Target(boardState);
            boardState.Event(GameStateEvents.ExitBoardEvent).Target(mapState);
            mapState.Event(GameStateEvents.DisposedEvent).Target(editor.Final());
        }

        public void Dispose()
        {
            signalBus.Unsubscribe<ChangeLevelSignal>(OnLevelChange);
            signalBus.Unsubscribe<ExitToMapSignal>(OnExitToMap);
        }
    }
}