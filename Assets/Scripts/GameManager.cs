using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int Attempts { get; private set; } = 0;
    public Timer Clock { get; private set; }
    public string ClockTime => TimeUtils.ElapsedTimeToDisplay(Clock.ElapsedTime);

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Clock = new Timer(480); // Will tick for 8 in-game hours.
    }

    private void Start()
    {
        Attempts++;
        Clock.Start();
    }

    private void Update()
    {
        Clock.Tick(Time.deltaTime);
    }

    public void EndAttempt()
    {
        // @TODO
    }

    public void Win()
    {
        Debug.Log("All tasks completed. You win!");
    }
}
