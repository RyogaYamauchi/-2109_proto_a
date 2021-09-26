using App.Common;
using App.Presenters;
using Zenject;

public class RootInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<TitleRootPresenter>().AsTransient();
        Container.Bind<MainRootPresenter>().AsTransient();
        Container.Bind<ResultRootPresenter>().AsTransient();
        Container.Bind<CommonSceneManager>().AsSingle();
    }
}