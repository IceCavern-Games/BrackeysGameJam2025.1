using Reflex.Attributes;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameplayUIManager Gameplay => _gameplay;

    [Inject] private readonly GameManager _gameManager;

    [SerializeField] private GameplayUIManager _gameplay;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Debug.Assert(_gameplay != null, "Gameplay UI Manager not set!");
    }

    private void Update()
    {
        _gameplay.SetClockText(_gameManager.ClockTime);
    }

    /// <summary>
    /// Hide all UI elements.
    /// </summary>
    public void HideAllUI()
    {
        _gameplay.gameObject.SetActive(false);
    }

    /// <summary>
    /// Show the Gameplay UI.
    /// </summary>
    public void ShowGameplay(bool show = true)
    {
        _gameplay.gameObject.SetActive(show);
    }
}
