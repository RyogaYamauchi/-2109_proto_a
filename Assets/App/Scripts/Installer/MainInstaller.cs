using App.Presenters;
using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<TitleRootPresenter>().AsTransient();
    }
}