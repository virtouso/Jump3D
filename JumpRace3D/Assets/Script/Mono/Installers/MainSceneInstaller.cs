using Zenject;

public class MainSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<BaseGameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<UiManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PoolManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameStateManager>().FromComponentInHierarchy().AsSingle();

    }
}