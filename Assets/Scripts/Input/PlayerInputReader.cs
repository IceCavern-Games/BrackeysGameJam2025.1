using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool interact;
    public bool paint;
    public bool erase;
    public bool pause;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorInputForLook = true;

    [Inject] private readonly GameManager _gameManager;
    [Inject] private readonly InputManager _input;

    private InputActionAsset _actions;

    private void OnEnable()
    {
        _actions = _input.Input.actions;

        _actions["Move"].performed += ctx => move = ctx.ReadValue<Vector2>();
        _actions["Move"].canceled += ctx => move = Vector2.zero;

        _actions["Look"].performed += ctx =>
        {
            if (cursorInputForLook)
                look = ctx.ReadValue<Vector2>();
        };
        _actions["Look"].canceled += ctx => look = Vector2.zero;

        _actions["Jump"].performed += ctx => jump = true;
        _actions["Jump"].canceled += ctx => jump = false;

        _actions["Sprint"].performed += ctx => sprint = true;
        _actions["Sprint"].canceled += ctx => sprint = false;

        _actions["Interact"].performed += ctx => interact = true;
        _actions["Interact"].canceled += ctx => interact = false;

        _actions["Paint"].performed += ctx => paint = true;
        _actions["Paint"].canceled += ctx => paint = false;

        _actions["Erase"].performed += ctx => erase = true;
        _actions["Erase"].canceled += ctx => erase = false;

        _actions["Pause"].performed += ctx => pause = true;
        _actions["Pause"].canceled += ctx => pause = false;
    }

    private void OnDisable()
    {
        _actions["Move"].performed -= ctx => move = ctx.ReadValue<Vector2>();
        _actions["Move"].canceled -= ctx => move = Vector2.zero;

        _actions["Look"].performed -= ctx =>
        {
            if (cursorInputForLook)
                look = ctx.ReadValue<Vector2>();
        };
        _actions["Look"].canceled -= ctx => look = Vector2.zero;

        _actions["Jump"].performed -= ctx => jump = true;
        _actions["Jump"].canceled -= ctx => jump = false;

        _actions["Sprint"].performed -= ctx => sprint = true;
        _actions["Sprint"].canceled -= ctx => sprint = false;

        _actions["Interact"].performed -= ctx => interact = true;
        _actions["Interact"].canceled -= ctx => interact = false;

        _actions["Paint"].performed -= ctx => paint = true;
        _actions["Paint"].canceled -= ctx => paint = false;

        _actions["Erase"].performed -= ctx => erase = true;
        _actions["Erase"].canceled -= ctx => erase = false;

        _actions["Pause"].performed -= ctx => pause = true;
        _actions["Pause"].canceled -= ctx => pause = false;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (_gameManager.Pause.IsOpen)
            hasFocus = false;

        _input.SetCursorLock(hasFocus);
    }
}
