using Reflex.Core;
using UnityEngine;

public class GameServiceProvider : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        // Instantiate Global Manager.
        //

        // Bind Global Managers.
        //

        // Build the container.
        var container = builder.Build();

        // Inject any of the top-level manager that have co-dependencies.
        //
    }
}
