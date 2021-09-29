using App.Presentation;
using Zenject;

namespace App.DI
{
    public class MainInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<BattlePresenter>().AsTransient();
        }
    }
}