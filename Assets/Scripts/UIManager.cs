using Reflex.Attributes;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameplayUIManager Gameplay => _gameplay;
    public PauseScreenManager Pause => _pause;

    [Inject] private readonly GameManager _gameManager;

    [SerializeField] private GameplayUIManager _gameplay;
    [SerializeField] private PauseScreenManager _pause;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Debug.Assert(_gameplay != null, "Gameplay UI Manager not set!");
        Debug.Assert(_pause != null, "Pause Screen Manager is not set!");
    }

    private void Update()
    {
        _gameplay.SetClockText(_gameManager.ClockTime);
    }

    public void EnablePauseScreen()
    {
        _pause.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide all UI elements.
    /// </summary>
    public void HideAllUI()
    {
        _gameplay.gameObject.SetActive(false);
        _pause.gameObject.SetActive(false);
    }

    /// <summary>
    /// Show the Gameplay UI.
    /// </summary>
    public void ShowGameplay(bool show = true)
    {
        _gameplay.gameObject.SetActive(show);
    }
}
