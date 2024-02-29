using App.StateMachine;
using Core.Utils;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace App.GameStates.States
{
   
    [UsedImplicitly]
    public class BootState : IState
    {
        [Inject]
        private CoroutineProvider coroutineProvider;
        
        public void OnEnter()
        {
            Debug.Log("Booting the game!");
        }

        public void OnExit()
        {
            Debug.Log("Booting is complete!");
        }
    }
}