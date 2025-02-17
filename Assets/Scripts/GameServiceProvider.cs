using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

public class GameServiceProvider : MonoBehaviour, IInstaller
{
    [SerializeField] private GameObject _uiManagerPrefab;

    public void InstallBindings(ContainerBuilder builder)
    {
        // Instantiate Global Manager.
        GameObject uiManagerInstance = Instantiate(_uiManagerPrefab);

        // Bind Global Managers.
        builder.AddSingleton(uiManagerInstance.GetComponent<UIManager>(), typeof(UIManager));

        // Build the container.
        var container = builder.Build();

        // Inject any of the top-level manager that have co-dependencies.
        GameObjectInjector.InjectRecursive(uiManagerInstance, container);
    }
}
