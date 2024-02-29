using Features.Signals;
using UnityEngine;
using Zenject;

namespace Features.Views
{
    public class ExitButtonView : MonoBehaviour
    {
        [Inject] 
        private SignalBus signalBus;
		
        
        public void OnExitClick()
        {
            signalBus.Fire(new ExitToMapSignal());
        }
    }
}