using Reflex.Attributes;
using System;
using System.Collections;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Inject] private readonly GameManager _gameManager;
    [Inject] private readonly InputManager _inputManager;

    private void Start()
    {
        _inputManager.FindPlayerInput();
        _inputManager.Input.DeactivateInput();

        // Easy way of just starting an attempt every time the scene loads.
        // @TODO: Will probably eventually make this trigger after a fade out/in or whatever.
        StartCoroutine(CoroutineUtils.WaitOneFrame(() => { _gameManager.StartAttempt(); }));
    }
}
