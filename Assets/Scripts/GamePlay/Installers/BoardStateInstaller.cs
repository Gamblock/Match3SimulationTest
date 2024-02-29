using Features.Commands;
using Features.Signals;
using GamePlay.Signals;
using Zenject;

namespace GamePlay.Installers
{
    public class BoardStateInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            //Signals
            Container.DeclareSignal<Match3Signals.OnBoardCreatedSignal>();
            Container.DeclareSignal<Match3Signals.StartSwapSignal>();
            Container.DeclareSignal<Match3Signals.FindMatchesSignal>();
            Container.DeclareSignal<Match3Signals.OnPlayerTurnStart>();
            Container.DeclareSignal<Match3Signals.TileCreatedSignal>();
            Container.DeclareSignal<Match3Signals.SwapResultSignal>();
            Container.DeclareSignal<Match3Signals.CalculateMatchesSignal>();
            
            //High-level controllers
            Container.BindInterfacesAndSelfTo<BoardModelHandler>().AsSingle();
            
            Container.BindSignal<Match3Signals.CreateBoardSignal>()
                .ToMethod<BoardModelHandler>(x => x.CreateBoard).FromResolve();
            
            Container.BindSignal<Match3Signals.CalculateMatchesSignal>()
                .ToMethod<BoardModelHandler>(x => x.HandleSwap).FromResolve();
            
            
        }
    }
}