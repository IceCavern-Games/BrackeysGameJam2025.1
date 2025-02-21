using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Inject] private readonly DialogueManager _dialogueManager;
    [Inject] private readonly GameManager _gameManager;

    public PlayerInput Input { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void FindPlayerInput()
    {
        // Find the player's input module.
        var player = FindFirstObjectByType<PlayerInput>();

        Debug.Assert(player != null, "Player could not be found!");

        Input = player;
    }

    public void SetCursorLock(bool isLocked = true)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
