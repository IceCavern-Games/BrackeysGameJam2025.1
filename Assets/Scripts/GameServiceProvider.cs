using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

public class GameServiceProvider : MonoBehaviour, IInstaller
{
    [SerializeField] private GameObject _audioManagerPrefab;
    [SerializeField] private GameObject _dialogueManagerPrefab;
    [SerializeField] private GameObject _gameManagerPrefab;
    [SerializeField] private GameObject _inputManagerPrefab;
    [SerializeField] private GameObject _taskManagerPrefab;
    [SerializeField] private GameObject _uiManagerPrefab;
    [SerializeField] private GameObject _paintTextureManagerPrefab;

    public void InstallBindings(ContainerBuilder builder)
    {
        // Instantiate Global Manager.
        GameObject audioManagerInstance = Instantiate(_audioManagerPrefab);
        GameObject dialogueManagerInstance = Instantiate(_dialogueManagerPrefab);
        GameObject gameManagerInstance = Instantiate(_gameManagerPrefab);
        GameObject inputManagerInstance = Instantiate(_inputManagerPrefab);
        GameObject taskManagerInstance = Instantiate(_taskManagerPrefab);
        GameObject uiManagerInstance = Instantiate(_uiManagerPrefab);
        GameObject paintTextureManagerInstance = Instantiate(_paintTextureManagerPrefab);

        // Bind Global Managers.
        builder.AddSingleton(audioManagerInstance.GetComponent<AudioManager>(), typeof(AudioManager));
        builder.AddSingleton(dialogueManagerInstance.GetComponent<DialogueManager>(), typeof(DialogueManager));
        builder.AddSingleton(gameManagerInstance.GetComponent<GameManager>(), typeof(GameManager));
        builder.AddSingleton(inputManagerInstance.GetComponent<InputManager>(), typeof(InputManager));
        builder.AddSingleton(taskManagerInstance.GetComponent<TaskManager>(), typeof(TaskManager));
        builder.AddSingleton(uiManagerInstance.GetComponent<UIManager>(), typeof(UIManager));
        builder.AddSingleton(paintTextureManagerInstance.GetComponent<PaintTextureManager>(), typeof(PaintTextureManager));

        // Build the container.
        var container = builder.Build();

        // Inject any of the top-level manager that have co-dependencies.
        GameObjectInjector.InjectRecursive(audioManagerInstance, container);
        GameObjectInjector.InjectRecursive(dialogueManagerInstance, container);
        GameObjectInjector.InjectRecursive(inputManagerInstance, container);
        GameObjectInjector.InjectRecursive(taskManagerInstance, container);
        GameObjectInjector.InjectRecursive(uiManagerInstance, container);
        GameObjectInjector.InjectRecursive(paintTextureManagerInstance, container);

        GameObjectInjector.InjectRecursive(gameManagerInstance, container);
    }
}
