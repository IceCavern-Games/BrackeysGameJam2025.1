using Reflex.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Inject] private readonly DialogueManager _dialogueManager;
    [Inject] private readonly InputManager _inputManager;
    [Inject] private readonly TaskManager _taskManager;

    public int Attempts { get; private set; } = 0;
    public Timer Clock { get; private set; }
    public string ClockTime => TimeUtils.ElapsedTimeToDisplay(Clock.ElapsedTime);

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Clock = new Timer(480, false); // Will tick for 8 in-game hours.
    }

    private void Update()
    {
        Clock.Tick(Time.deltaTime);
    }

    public void EndAttempt()
    {
        Clock.Stop();
        Clock.Reset();
        _taskManager.Reset();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Fail()
    {
        _dialogueManager.StartConversation("Fail", () =>
        {
            EndAttempt();
        });
    }

    public void StartAttempt()
    {
        _dialogueManager.StartConversation("Intro", () =>
        {
            Attempts++;
            Clock.Start();
        });
    }

    public void Win()
    {
        _dialogueManager.StartConversation("Win", () =>
        {
            // @TODO: Idk, end the game or whatever.
            EndAttempt();
        });
    }
}
