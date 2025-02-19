using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
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
}
