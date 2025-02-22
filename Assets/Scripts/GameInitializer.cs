using Reflex.Attributes;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Inject] private readonly GameManager _gameManager;
    [Inject] private readonly InputManager _inputManager;
    [Inject] private readonly UIManager _uiManager;

    private void Start()
    {
        _inputManager.Input.DeactivateInput();
        _inputManager.Prompts.ShowPrompts(PromptMappings.Gameplay);
        _inputManager.SetCursorLock(true);
        _uiManager.ShowGameplay();
        _uiManager.EnablePauseScreen();

        // Easy way of just starting an attempt every time the scene loads.
        // @TODO: Will probably eventually make this trigger after a fade out/in or whatever.
        StartCoroutine(CoroutineUtils.WaitOneFrame(() => { _gameManager.StartAttempt(); }));
    }
}
