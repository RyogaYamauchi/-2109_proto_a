using App.Application;
using App.Data;
using App.Domain;
using App.Presentation;
using Zenject;

namespace App.DI
{
    public class RootInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Models
            Container.Bind<GameModel>().AsSingle();
            
            // Presenters
            Container.Bind<TitleRootPresenter>().AsTransient();
            Container.Bind<MainRootPresenter>().AsTransient();
            Container.Bind<ResultRootPresenter>().AsTransient();
            Container.Bind<CommonSceneManager>().AsSingle();
            Container.Bind<MatchingRootPresenter>().AsTransient();

            Container.Bind<BattlePresenter>().AsTransient();
            
            // Repositories
            Container.Bind<SkillRepository>().AsSingle();
            Container.Bind<TsumuRepository>().AsSingle();
            
            // UseCases
            Container.Bind<MainRootUseCase>().AsTransient();
            Container.Bind<BattleUseCase>().AsTransient();
            
            // Services
            Container.Bind<SkillService>().AsSingle();
        }
    }
}