using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

public class GameServiceProvider : MonoBehaviour, IInstaller
{
    [SerializeField] private GameObject _audioManagerPrefab;
    [SerializeField] private GameObject _dialogueManagerPrefab;
    [SerializeField] private GameObject _inputManagerPrefab;
    [SerializeField] private GameObject _uiManagerPrefab;

    public void InstallBindings(ContainerBuilder builder)
    {
        // Instantiate Global Manager.
        GameObject audioManagerInstance = Instantiate(_audioManagerPrefab);
        GameObject dialogueManagerInstance = Instantiate(_dialogueManagerPrefab);
        GameObject inputManagerInstance = Instantiate(_inputManagerPrefab);
        GameObject uiManagerInstance = Instantiate(_uiManagerPrefab);

        // Bind Global Managers.
        builder.AddSingleton(audioManagerInstance.GetComponent<AudioManager>(), typeof(AudioManager));
        builder.AddSingleton(dialogueManagerInstance.GetComponent<DialogueManager>(), typeof(DialogueManager));
        builder.AddSingleton(inputManagerInstance.GetComponent<InputManager>(), typeof(InputManager));
        builder.AddSingleton(uiManagerInstance.GetComponent<UIManager>(), typeof(UIManager));

        // Build the container.
        var container = builder.Build();

        // Inject any of the top-level manager that have co-dependencies.
        GameObjectInjector.InjectRecursive(audioManagerInstance, container);
        GameObjectInjector.InjectRecursive(dialogueManagerInstance, container);
        GameObjectInjector.InjectRecursive(inputManagerInstance, container);
        GameObjectInjector.InjectRecursive(uiManagerInstance, container);
    }
}
